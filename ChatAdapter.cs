using Android.Content;
using Android.Views;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    public class ChatAdapter : BaseAdapter<Chat>
    {
        private readonly Activity _activity;

        private readonly List<Chat> _chats;

        public ChatAdapter(Activity activity, List<Chat> chats)
        {
            _activity = activity;
            _chats = chats;
        }

        public override Chat this[int position] => _chats[position];

        public override int Count => _chats.Count;

        public override long GetItemId(int position) => position;

        public override View? GetView(int position, View? convertView, ViewGroup? parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.ChatItem, null);

            var chatNameTextView = view.FindViewById<TextView>(Resource.Id.chatName);
            var lastMessageTextView = view.FindViewById<TextView>(Resource.Id.lastMessageText);
            var lastMessageDateTextView = view.FindViewById<TextView>(Resource.Id.lastMessageDate);
            var lastMessageSenderNameTextView = view.FindViewById<TextView>(Resource.Id.lastMessageSenderName);
            var dots = view.FindViewById<TextView>(Resource.Id.dots);

            var chat = _chats[position];
            var lastMessage = chat.Messages.LastOrDefault();

            chatNameTextView.Text = chat.Name;
            lastMessageTextView.Text = lastMessage?.Text;
            var messageDate = lastMessage.Date;
            var nowDate = DateTime.Now;

            lastMessageDateTextView.Text =
                messageDate.Day == nowDate.Day && messageDate.Month == nowDate.Month && messageDate.Year == nowDate.Year
                ? messageDate.ToShortTimeString()
                : messageDate.ToString();
            lastMessageSenderNameTextView.Text = lastMessage?.Sender;
            dots.Text = lastMessage is null ? "" : ": ";

            return view;
        }
    }
}
