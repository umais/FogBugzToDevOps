using System;
using System.Collections.Generic;
using System.Text;

namespace DevOpsImporter.Entities
{
    public class WorkItem
    {
        public static Dictionary<string, string> EmployeeList = new Dictionary<string, string>() 
        {
            {"5","Steve Attwell" },
            {"4","Umais Siddiqui" },
            {"24","Stephen Jackson" },
            {"46" ,"Sameh Abdulfutooh"},
            {"52","Saddam Khan" },
            {"8","Muhammad Siddiqui" },
            {"16","Muhammad Junaid" },
            {"26","Michael Ross" },
            {"9","Jo Anne Lutz" },
            {"25","Jana Sanford" },
            {"22","Fred Goldwater" },
            {"28","Conie Agno" },
            {"38","Azure Admin" },
            {"6","Arif Khan" },
            {"23","Amir Saleem" },
            {"1","Closed" },
             {"48","Dereck Gilbert" },
        };

        public WorkItem()
        {
           
        }
        public String Id { get; set; }
        public String Title { get; set; }
        public String Status { get; set; }

        public String LatestTextSummary { get; set; }

        public String Category { get; set; }

        public String Milestone { get; set; }

        public String Area { get; set; }

        public String ProjectName { get; set; }

        public String AssignedTo { get; set
                ; }

        public string Priority { get; set; }
            
        public List<Event> EventList { get; set; }
    }
}
