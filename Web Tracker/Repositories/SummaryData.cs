using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebTracker.Repositories;
using SignalR_Check.Models;
using Microsoft.Data.SqlClient;

namespace WebTracker.Repositories
{
    public class SummaryData : ISummaryData
    {
        public List<FlowSummary> GetAllFlows(int loggedInUserWebsiteId)
        {
            List<FlowSummary> flows=new List<FlowSummary>();
            string con = @"Data Source=63.250.53.44;Initial Catalog=webtracker;User ID=ahmad;Password=ahmad@1234;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;";
            string query = $"select * from SummaryTable where WebsiteID={loggedInUserWebsiteId}";
            SqlConnection connection = new SqlConnection(con);
            connection.Open();
            SqlCommand cmd=new SqlCommand(query, connection);
            SqlDataReader dr=cmd.ExecuteReader();
            while (dr.Read())
            {
                FlowSummary flow = new FlowSummary();
                flow.FlowSummed = dr[0].ToString();
                flow.Count = int.Parse(dr[2].ToString());
                flows.Add(flow);
            }
            return flows;
        }
        public List<ActionSummary> GetAllActions(int loggedInUserWebsiteId)
        {
            List<ActionSummary> flows = new List<ActionSummary>();
            string con = @"Data Source=63.250.53.44;Initial Catalog=webtracker;User ID=ahmad;Password=ahmad@1234;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;";
            string query = $"select * from ActionSummaryTable where WebsiteID={loggedInUserWebsiteId}";
            SqlConnection connection = new SqlConnection(con);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ActionSummary flow = new ActionSummary();
                flow.ActionSummed = dr[0].ToString();
                flow.Count = int.Parse(dr[2].ToString());
                flows.Add(flow);
            }
            return flows;
        }
    }
}

