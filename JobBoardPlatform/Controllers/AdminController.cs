using JobBoardPlatform.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

public class AdminController : Controller
{
    string conStr = ConfigurationManager.ConnectionStrings["JobBoardDB"].ConnectionString;

    // ✅ 1. Admin Dashboard - Show Unapproved Jobs
    public ActionResult Dashboard()
    {
        if (Session["UserRole"]?.ToString() != "Admin")
            return RedirectToAction("Login", "Account");

        var model = new AdminDashboardViewModel();
        model.PendingJobs = new List<Job>();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();

            // Total Jobs
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Jobs", con))
                model.TotalJobs = (int)cmd.ExecuteScalar();

            // Pending Jobs Count
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Jobs WHERE IsApproved = 0", con))
                model.PendingJobsCount = (int)cmd.ExecuteScalar();

            // Total Applications
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Applications", con))
                model.TotalApplications = (int)cmd.ExecuteScalar();

            // Approved Applications
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Applications WHERE Status = 'Approved'", con))
                model.ApprovedApplications = (int)cmd.ExecuteScalar();

            // Rejected Applications
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Applications WHERE Status = 'Rejected'", con))
                model.RejectedApplications = (int)cmd.ExecuteScalar();

            // Total Employers
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Role = 'Employer'", con))
                model.TotalEmployers = (int)cmd.ExecuteScalar();

            // List of Pending Jobs
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM Jobs WHERE IsApproved = 0", con))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.PendingJobs.Add(new Job
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            PostedDate = Convert.ToDateTime(reader["PostedDate"])
                        });
                    }
                }
            }
        }

        return View(model);
    }




    // ✅ 2. Approve Job
    public ActionResult Approve(int id)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "UPDATE Jobs SET IsApproved = 1 WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Dashboard");
    }

    // ✅ 3. Reject Job
    public ActionResult Reject(int id)
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = "DELETE FROM Jobs WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction("Dashboard");
    }

    // ✅ 4. View New Applications Only (Status = Applied)
    public ActionResult Applications()
    {
        List<Application> apps = new List<Application>();
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"SELECT A.Id, A.AppliedDate, A.Status, A.ResumePath, 
                             J.Title AS JobTitle, U.Name AS SeekerName, E.Name AS EmployerName
                             FROM Applications A
                             JOIN Jobs J ON A.JobId = J.Id
                             JOIN Users U ON A.UserId = U.Id
                             JOIN Users E ON J.PostedBy = E.Id
                             WHERE A.Status = 'Applied'";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                apps.Add(new Application
                {
                    Id = (int)dr["Id"],
                    AppliedDate = (DateTime)dr["AppliedDate"],
                    Status = dr["Status"].ToString(),
                    ResumePath = dr["ResumePath"].ToString(),
                    CandidateName = dr["SeekerName"].ToString(),
                    JobTitle = dr["JobTitle"].ToString(),
                    EmployerName = dr["EmployerName"].ToString()
                });
            }
        }

        return View(apps); // View: Views/Admin/Applications.cshtml
    }

    // ✅ 5. View All Applications (No filter)
    public ActionResult ViewApplications()
    {
        List<Application> apps = new List<Application>();
        using (SqlConnection con = new SqlConnection(conStr))
        {
            string query = @"
            SELECT A.Id, A.AppliedDate, A.Status, A.ResumePath, 
                   J.Title AS JobTitle, 
                   E.Name AS EmployerName, 
                   U.Name AS CandidateName
            FROM Applications A
            JOIN Jobs J ON A.JobId = J.Id
            JOIN Users U ON A.UserId = U.Id
            JOIN Users E ON J.PostedBy = E.Id";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                apps.Add(new Application
                {
                    Id = (int)dr["Id"],
                    AppliedDate = (DateTime)dr["AppliedDate"],
                    Status = dr["Status"].ToString(),
                    ResumePath = dr["ResumePath"].ToString(),
                    JobTitle = dr["JobTitle"].ToString(),
                    EmployerName = dr["EmployerName"].ToString(),
                    CandidateName = dr["CandidateName"].ToString()
                });
            }
        }

        return View(apps); // View: Views/Admin/ViewApplications.cshtml
    }
}
