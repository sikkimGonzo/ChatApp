using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using ChatApp.Models;
using Newtonsoft.Json;
using System.Drawing;
using System.Net.Http.Headers;

namespace ChatApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : ListActivity
    {
        private List<Chat> _chats = new List<Chat>();
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayShowTitleEnabled(false);

            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeAsUpIndicator(Resource.Drawable.addChat);

            LayoutInflater inflator = LayoutInflater.From(this);
            View v = inflator.Inflate(Resource.Layout.ActionBar, null);

            ActionBar.LayoutParams layoutParams = new ActionBar.LayoutParams(ActionBar.LayoutParams.WrapContent,
                    ActionBar.LayoutParams.WrapContent, GravityFlags.Center);

            var titleView = v.FindViewById<TextView>(Resource.Id.action_bar_title);
            titleView.Text = "Chats";

            _chats = Read();
            ListAdapter = new ChatAdapter(this, _chats);

            ActionBar.SetCustomView(v, layoutParams);
            ActionBar.SetDisplayShowCustomEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    ShowDialog(0);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            _chats = Read();
            ListAdapter = new ChatAdapter(this, _chats);
        }

        protected override Dialog OnCreateDialog(int id)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Enter chat name");

            EditText input = new EditText(this);
            builder.SetView(input);

            builder.SetPositiveButton("OK", delegate {
                string chatName = input.Text.Trim();
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
                        ListAdapter = new ChatAdapter(this, _chats);
                    }
                }
                input.Text = "";
            });

            builder.SetNegativeButton("Cancel", delegate { });

            return builder.Create();
        }

        protected override void OnListItemClick(ListView l, Android.Views.View v, int position, long id)
        {
            var n = _chats[(int)id];
            var t = ListAdapter.GetItem(position).ToString();
            var chatActivity = new Intent(this, typeof(ChatActivity));
            chatActivity.PutExtra("ChatName", n.Name);
            StartActivity(chatActivity);
        }

        public void Save(Chat chat)
        {
            string path = Android.App.Application.Context.FilesDir.Path;
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

        public List<Chat> Read()
        {
            string path = Android.App.Application.Context.FilesDir.Path;

            var chats = new List<Chat>();

            var files = Directory.EnumerateFiles(path);

            //foreach (var file in files)
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
                        chats.Add(chat);
                    }
                }
            }
            return chats;
        }
    }
}