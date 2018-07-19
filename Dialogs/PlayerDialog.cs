using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;


namespace FoosRanking
{
    [Serializable]
    public class PlayerDialog : IDialog<object>
    {
        protected int messageNumber = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the scores?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            if (message.Text == "scores")
            {
                var key = 1;
                string data;
                while (context.ConversationData.TryGetValue("Message:" + key++, out data))
                    await context.PostAsync($"Message:{key}={data}");
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                context.ConversationData.SetValue("Message:" + messageNumber, message.Text);
                await context.PostAsync($"{messageNumber++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                context.ConversationData.Clear();
                await context.PostAsync("Reset scores.");
            }
            else
            {
                await context.PostAsync("Did not reset scores.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}