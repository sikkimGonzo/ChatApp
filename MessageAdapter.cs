using Android.Views;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    public class MessageAdapter : BaseAdapter<Message>
    {
        private readonly Activity _activity;

        private readonly List<Message> _messages;

        public MessageAdapter(Activity activity, List<Message> messages)
        {
            _activity = activity;
            _messages = messages;
        }

        public override Message this[int position] => _messages[position];

        public override int Count => _messages.Count;

        public override long GetItemId(int position) => position;

        public override View? GetView(int position, View? convertView, ViewGroup? parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.MessageItem, null);

            var messageTextView = view?.FindViewById<TextView>(Resource.Id.messageText);
            var messageDateTextView = view?.FindViewById<TextView>(Resource.Id.messageDate);
            var messageSenderTextView = view?.FindViewById<TextView>(Resource.Id.senderName);

            var message = _messages[position];

            if(messageTextView is not null)
            {
                messageTextView.Text = message.Text;
            }
            if(messageDateTextView is not null)
            {
                if (message.Date is not null)
                {
                    var messageDate = (DateTime)message.Date;
                    var nowDate = DateTime.Now;
                    messageDateTextView.Text =
                        messageDate.Day == nowDate.Day && messageDate.Month == nowDate.Month && messageDate.Year == nowDate.Year
                        ? messageDate.ToShortTimeString()
                        : messageDate.ToString();
                }
            }
            if(messageSenderTextView is not null)
            {
                messageSenderTextView.Text = message.Sender;
            }

            return view;
        }

        public void Add(Message message)
        {
            _messages.Add(message);
            NotifyDataSetChanged();
        }
    }
}
