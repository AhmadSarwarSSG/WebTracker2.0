using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalRCheck;
using Microsoft.EntityFrameworkCore;
using WebTracker.Repositories;
using SignalR_Check.Models;
using Microsoft.Data.SqlClient;

namespace WebTracker.Repositories
{
    public class SummaryData : ISummaryData
    {
        public List<FlowSummary> GetAllFlows()
        {
            List<FlowSummary> flows=new List<FlowSummary>();
            string con = @"Data Source=(localdb)\ProjectModels;Initial Catalog=WebTracker;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string query = "select * from SummaryTable";
            SqlConnection connection = new SqlConnection(con);
            connection.Open();
            SqlCommand cmd=new SqlCommand(query, connection);
            SqlDataReader dr=cmd.ExecuteReader();
            while (dr.Read())
            {
                FlowSummary flow = new FlowSummary();
                flow.FlowSummed = dr[0].ToString();
                flow.Count = int.Parse(dr[1].ToString());
                flows.Add(flow);
            }
            return flows;
        }
    }
}

