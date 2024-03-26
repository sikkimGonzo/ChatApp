using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }

        public string? Sender { get; set; }

        public DateTime? Date { get; set; }

        public string? Text { get; set; }
    }
}
