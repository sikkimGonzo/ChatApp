using Android.Views;
using Android.Widget;
using ChatApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.Apache.Http.Conn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    [Activity(Label = "ChatActivity")]
    public class ChatActivity : Activity
    {
        private List<Message>? _messages = new List<Message>();

#pragma warning disable CS8765 // Допустимость значений NULL для типа параметра не соответствует переопределенному элементу (возможно, из-за атрибутов допустимости значений NULL).
        protected override void OnCreate(Bundle bundle)
#pragma warning restore CS8765 // Допустимость значений NULL для типа параметра не соответствует переопределенному элементу (возможно, из-за атрибутов допустимости значений NULL).
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Chat);

            ActionBar?.SetDisplayShowHomeEnabled(false);
            ActionBar?.SetDisplayHomeAsUpEnabled(true);
            ActionBar?.SetDisplayShowTitleEnabled(false);

            var inflator = LayoutInflater.From(this);
            var v = inflator?.Inflate(Resource.Layout.ActionBar, null);

            var layoutParams = new ActionBar.LayoutParams(ActionBar.LayoutParams.WrapContent,
                    ActionBar.LayoutParams.WrapContent, GravityFlags.Center);

            var chatName = Intent?.Extras?.GetString("ChatName");
            var titleView = v?.FindViewById<TextView>(Resource.Id.action_bar_title);
            if (titleView is not null)
            {
                titleView.Text = chatName;
            }
            ActionBar?.SetCustomView(v, layoutParams);
            ActionBar?.SetDisplayShowCustomEnabled(true);

            var sendButton = FindViewById<ImageButton>(Resource.Id.sendButton);
            var messageInput = FindViewById<EditText>(Resource.Id.messageInput);
            var messageList = FindViewById<ListView>(Resource.Id.messageList);

            if (chatName is not null)
            {
                _messages = Read(chatName)?.Messages?.OrderBy(x => x.Date)?.ToList();
            }

            if (_messages is not null && messageList is not null)
            {
                var adapter = new MessageAdapter(this, _messages);
                messageList.Adapter = adapter;
                ScrollMessagesToEnd(messageList);


                if (sendButton is not null)
                {
                    sendButton.Click += delegate
                    {
                        var messageText = messageInput?.Text?.Trim();
                        if (!string.IsNullOrEmpty(messageText))
                        {
                            var userName = Resources?.GetString(Resource.String.user_name);
                            var message = new Message
                            {
                                Text = messageText,
                                Sender = userName ?? "",
                                Date = DateTime.Now
                            };

                            if (chatName is not null)
                            {
                                adapter.Add(message);
                                adapter.NotifyDataSetChanged();
                                Save(chatName, message);
                                if (messageInput is not null)
                                {
                                    messageInput.Text = "";
                                }

                                var botMessage = GetBotMessage();
                                adapter.Add(botMessage);
                                adapter.NotifyDataSetChanged();
                                Save(chatName, botMessage);

                                ScrollMessagesToEnd(messageList);
                            }
                        }
                    };
                }
            }
        }

        public Message GetBotMessage()
        {
            var botName = Resources?.GetString(Resource.String.bot_name);
            var botAnswers = Resources?.GetStringArray(Resource.Array.bot_answers);
            var randomBotAnswer = "";
            if (botAnswers is not null)
            {
                randomBotAnswer = botAnswers[new Random().Next(botAnswers.Length)];
            }
            return new Message
            {
                Text = randomBotAnswer,
                Sender = botName ?? "",
                Date = DateTime.Now
            };
        }

        public void ScrollMessagesToEnd(ListView messageList)
        {
            messageList.Post(() =>
            {
                messageList.SetSelection(messageList.Count - 1);
            });
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void Save(string chatName, Message message)
        {
            if (Application.Context.FilesDir is not null)
            {
                var path = Application.Context.FilesDir.Path;
                var backingFile = Path.Combine(path, $"{chatName}.json");
                var chat = Read(chatName);
                chat?.Messages?.Add(message);
                using (var writer = File.CreateText(backingFile))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(chat));
                }
            }
        }

        public Chat? Read(string chatName)
        {
            var chat = new Chat();
            if (Application.Context.FilesDir is not null)
            {
                string path = Application.Context.FilesDir.Path;
                var backingFile = Path.Combine(path, $"{chatName}.json");

                if (backingFile == null || !File.Exists(backingFile))
                {
                    return new Chat();
                }

                string f = "";
                using (var reader = new StreamReader(backingFile, true))
                {
                    f = reader.ReadToEnd();
                }
                chat = JsonConvert.DeserializeObject<Chat>(f);
            }
            return chat;
        }
    }
}
