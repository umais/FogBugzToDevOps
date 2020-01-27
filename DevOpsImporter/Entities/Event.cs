using System;
using System.Collections.Generic;
using System.Text;

namespace DevOpsImporter.Entities
{
   public class Event
    {
        public int EventId { get; set; }

        public int ItemId { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; }

        public string EventType { get; set; }

        public string EventText { get; set; }

        public List<Attachments> CaseAttachments { get; set; }
         

    }
}
