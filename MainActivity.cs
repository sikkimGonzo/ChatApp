using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using ChatApp.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Net.Http.Headers;
using static Android.Icu.Text.Transliterator;

namespace ChatApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private List<Chat> _chats = new List<Chat>();
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            ActionBar?.SetDisplayShowHomeEnabled(false);
            ActionBar?.SetDisplayShowTitleEnabled(false);

            ActionBar?.SetHomeButtonEnabled(true);
            ActionBar?.SetDisplayHomeAsUpEnabled(true);
            ActionBar?.SetHomeAsUpIndicator(Resource.Drawable.addChat);

            var inflator = LayoutInflater.From(this);
            var v = inflator?.Inflate(Resource.Layout.ActionBar, null);

            var layoutParams = new ActionBar.LayoutParams(ActionBar.LayoutParams.WrapContent,
                    ActionBar.LayoutParams.WrapContent, GravityFlags.Center);

            ActionBar?.SetCustomView(v, layoutParams);
            ActionBar?.SetDisplayShowCustomEnabled(true);

            var titleView = v?.FindViewById<TextView>(Resource.Id.action_bar_title);
            if (titleView is not null)
            {
                titleView.Text = "Chats";
            }

            _chats = Read();
            var listView = FindViewById<ListView>(Resource.Id.list);
            if (listView is not null)
            {
                listView.Adapter = new ChatAdapter(this, _chats);

                listView.ItemClick += (sender, e) =>
                {
                    var chat = _chats[e.Position];
                    var chatActivity = new Intent(this, typeof(ChatActivity));
                    chatActivity.PutExtra("ChatName", chat.Name);
                    StartActivity(chatActivity);
                };
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
#pragma warning disable CS0618 // Тип или член устарел
                    ShowDialog(0);
#pragma warning restore CS0618 // Тип или член устарел
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            _chats = Read();
            var listView = FindViewById<ListView>(Resource.Id.list);
            if (listView is not null)
            {
                listView.Adapter = new ChatAdapter(this, _chats);
            }
        }

        [Obsolete]
        protected override Dialog? OnCreateDialog(int id)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Enter chat name");

            EditText input = new EditText(this);
            builder.SetView(input);

            builder.SetPositiveButton("OK", delegate {
                var chatName = input?.Text?.Trim();
                if (!string.IsNullOrEmpty(chatName))
                {
                    var isChatNameExist = _chats.Select(x => x.Name).Contains(chatName);
                    if (!isChatNameExist)
                    {
                        var chat = new Chat
                        {
                            Name = chatName,
                            Messages = new List<Message>()
                        };

                        _chats.Add(chat);
                        Save(chat);
                        var listView = FindViewById<ListView>(Resource.Id.list);
                        if (listView is not null)
                        {
                            listView.Adapter = new ChatAdapter(this, _chats);
                        }
                    }
                }
                if(input is not null)
                { 
                    input.Text = "";
                }
            });

            builder.SetNegativeButton("Cancel", delegate { });

            return builder.Create();
        }

        public void Save(Chat chat)
        {
            if (Application.Context.FilesDir is not null)
            {
                string path = Application.Context.FilesDir.Path;
                var backingFile = Path.Combine(path, $"{chat.Name}.json");
                if (!File.Exists(backingFile))
                {
                    File.Create(backingFile).Dispose();
                }
                using (var writer = File.CreateText(backingFile))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(chat));
                }
            }
        }

        public List<Chat> Read()
        {
            var chats = new List<Chat>();
            if (Application.Context.FilesDir is not null)
            {
                string path = Application.Context.FilesDir.Path;
                var files = Directory.EnumerateFiles(path);
                //foreach(var file in files)
                //{
                //    File.Delete(file);
                //}
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        using (var reader = new StreamReader(file, true))
                        {
                            var chat = JsonConvert.DeserializeObject<Chat>(reader.ReadToEnd());
                            if (chat is not null)
                            { 
                                chats.Add(chat);
                            }
                        }
                    }
                }
            }
            return chats;
        }
    }
}