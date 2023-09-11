using CryptocurrencyRateBot;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("5787517537:AAGiO3mfftfgYBx4F_mIrf5SfWS5JrKBoTI");
var cryptoRateController = new CryptoRateController();

using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();
  
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;

    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    if (message.Text is "/start")
    {
        Message sendHello = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Hey, let`s check the bitcoin rate in dollars, with /bitcoin_usd \n" +
                      "--- \n" +
                      "By this way, you can check the rate of any cryptocurrency in any currency! \n" +
                      "--- \n" +
                      "Also check /help to see more.",
                cancellationToken: cancellationToken);
    }
    else if (message.Text is "/ping")
    {
        string pingInfo = await cryptoRateController.GetPingInfo();

        Message sendPingInfo = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: pingInfo,
            cancellationToken: cancellationToken);
    }
    else if (message.Text is "/help")
    {
        Message sendHelp = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "This bot can send you the rate of any cryptovalue! \n" +
                      "--- \n" +
                      "To see rate, write this: \"/coinName_currencyName\" \n" +
                      "--- \n" +
                      "See server status: \"/ping\"",
                cancellationToken: cancellationToken);
    }
    else if (message.Text.Contains("_"))
    {
        string[] parts = message.Text.Split('_');

        if (parts.Length == 2)
        {
            string cryptoName = parts[0].Substring(1);
            string currencyName = parts[1];

            Console.WriteLine(cryptoName);
            Console.WriteLine(currencyName);

            string rate = await cryptoRateController.GetCryptoRate(cryptoName, currencyName);

            if (rate is "{}")
            {
                Message sentCoinNameError = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Smth went wrong!",
                cancellationToken: cancellationToken);
            }
            else
            {
                Message sentRate = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: rate,
                    cancellationToken: cancellationToken);
            }
        }
    }
    else
    {
        Message sentCheckHelp = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Check /help to see how it works!",
            cancellationToken: cancellationToken);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
