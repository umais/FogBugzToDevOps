using System;
using System.Collections.Generic;
using System.Text;

namespace DevOpsImporter.Entities
{
    public class Attachments
    {
        public int EventId { get; set; }
        public string AttachmentName { 
                                        get; 
                                        set; 
                                      }
        public string AttachmentURL { get; set; }
    }
}
