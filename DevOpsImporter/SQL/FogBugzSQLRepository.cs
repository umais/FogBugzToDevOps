using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DevOpsImporter.Entities;
namespace DevOpsImporter.SQL
{
    public sealed class FogBugzSQLRepository
    {
        private SqlConnection conn;
        private static FogBugzSQLRepository instance = null;
        public string connectionString { get; set; }
        public static FogBugzSQLRepository GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new FogBugzSQLRepository();
                return instance;
            }
        }
        private FogBugzSQLRepository()
        {
            if (!String.IsNullOrEmpty(connectionString))
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
            }
        }

        public void InsertWorkItem(WorkItem item)
        {
            if(conn==null)
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
            }
            using (var cmd = new SqlCommand("ItemsInsert", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@ItemId", System.Data.SqlDbType.Int);
                cmd.Parameters["@ItemId"].Value = Convert.ToInt32(item.Id);
                cmd.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@Title"].Value = item.Title;
                cmd.Parameters.Add("@ItemStatus", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@ItemStatus"].Value = item.Status;
                cmd.Parameters.Add("@LatestTextSummary", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@LatestTextSummary"].Value = item.LatestTextSummary;
                cmd.Parameters.Add("@Category", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@Category"].Value = item.Category;
                cmd.Parameters.Add("@Milestone", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@Milestone"].Value = item.Milestone;
                cmd.Parameters.Add("@Area", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@Area"].Value = item.Area;
                cmd.Parameters.Add("@ProjectName", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@ProjectName"].Value = item.ProjectName;
                cmd.Parameters.Add("@AssignedTo", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@AssignedTo"].Value = item.AssignedTo;
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertEvent(Event eventItem)
        {
            if (conn == null)
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
            }
            using (var cmd = new SqlCommand("ItemEventInsert", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@ItemId", System.Data.SqlDbType.Int);
                cmd.Parameters["@ItemId"].Value = Convert.ToInt32(eventItem.ItemId);
                cmd.Parameters.Add("@EventId", System.Data.SqlDbType.Int);
                cmd.Parameters["@EventId"].Value = Convert.ToInt32(eventItem.EventId);
                cmd.Parameters.Add("@EventName", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@EventName"].Value = eventItem.EventName;
                cmd.Parameters.Add("@EventDate", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@EventDate"].Value = eventItem.EventDate;
                cmd.Parameters.Add("@EventType", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@EventType"].Value = eventItem.EventType;
                cmd.Parameters.Add("@EventText", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@EventText"].Value = eventItem.EventText;
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertAttachments(Attachments attachment)
        {
            if (conn == null)
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
            }
            using (var cmd = new SqlCommand("EventAttachmentsInsert", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@EventId", System.Data.SqlDbType.Int);
                cmd.Parameters["@EventId"].Value = Convert.ToInt32(attachment.EventId);
                cmd.Parameters.Add("@AttachmentFileName", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@AttachmentFileName"].Value = attachment.AttachmentName;
                cmd.Parameters.Add("@AttachmentURL", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@AttachmentURL"].Value = attachment.AttachmentURL;
                cmd.ExecuteNonQuery();
            }


        }
    }
}
