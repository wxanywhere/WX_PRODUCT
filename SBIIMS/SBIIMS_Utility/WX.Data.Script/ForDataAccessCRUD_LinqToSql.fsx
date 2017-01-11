(* Copy form Layered Architecture Sample for .NET ExpenseSample

//--------------------------------------------------------------------------------
// Developed by: Serena Yeoh
// 
//--------------------------------------------------------------------------------
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//--------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using ExpenseSample.Business.Entities;
namespace ExpenseSample.Data
{
    public class ExpenseDAC
    {
        /// <summary>
        /// Inserts an expense row.
        /// </summary>
        /// <param name="expense">An Expense object.</param>
        public Expense Create(Expense expense)
        {
            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                dc.Expenses.InsertOnSubmit(expense);

                try
                {
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return expense;
            }

        }
        
        public void Insert(Expense expense)
        {
            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                dc.Expenses.InsertOnSubmit(expense);

                try
                {
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Updates an Expense row.
        /// </summary>
        /// <param name="expense">A Expense object.</param>
        public void UpdateStatus(Expense expense)
        {
            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                dc.Expenses.Attach(expense, true);

                try
                {
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Returns a set of Expenses
        /// </summary>
        /// <param name="status">A parameter array of Statuses</param>
        /// <returns>A List of expenses.</returns>
        public List<Expense> Select(params ExpenseStatus[] status)
        {
            string statusLine = BuildStatusLine(status);

            // NOTE: Recommended to use Stored Procedures instead.
            string sqlCmd = "SELECT * FROM [Expenses] " +
                (statusLine == string.Empty ? "" : "WHERE " + statusLine) +
                " ORDER BY [DateSubmitted] DESC";

            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                List<Expense> expensesList = dc.ExecuteQuery<Expense>(sqlCmd).ToList();
                return expensesList;
            }
        }

        /// <summary>
        /// Returns a set of Expenses that belongs to an employee
        /// </summary>
        /// <param name="employeeID">An EmployeeID.</param>
        /// <returns>A List of expenses.</returns>
        public List<Expense> SelectByEmployeeID(string employeeID)
        {
            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                List<Expense> expensesList =
                    (from expense in dc.Expenses
                     where expense.EmployeeID == employeeID
                     orderby expense.DateSubmitted descending 
                     select expense).ToList();

                return expensesList;
            }
        }

        public Expense SelectByID(Guid expenseID)
        {
            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                Expense result =
                    (from expense in dc.Expenses
                     where expense.ExpenseID == expenseID
                     select expense).First();

                return result;
            }
        }

        public List<Expense> SelectForHigherApproval()
        {
            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                List<Expense> expensesList =
                    (from expense in dc.Expenses
                     where ((expense.Category == ExpenseCategory.Medical &&
                             expense.Status == ExpenseStatus.Escalated) ||
                            (expense.Category != ExpenseCategory.Medical &&
                                (expense.Status == ExpenseStatus.Reviewed ||
                                 expense.Status == ExpenseStatus.Escalated)))
                     select expense).ToList();

                return expensesList;
            }
        }

        public List<Expense> SelectForMedicalApproval()
        {
            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                List<Expense> expensesList =
                    (from expense in dc.Expenses
                     where expense.Category == ExpenseCategory.Medical
                            && expense.Status == ExpenseStatus.Reviewed
                     select expense).ToList();

                return expensesList;
            }
        }

        private string BuildStatusLine(ExpenseStatus[] status)
        {
            string statusLine = string.Empty;

            // Build filter line.
            foreach (ExpenseStatus s in status)
            {
                statusLine += " ," + ((Byte)s).ToString();
            }

            if (!string.IsNullOrEmpty(statusLine))
                statusLine = " [Status] IN (" + statusLine.Remove(0, 2) + ") ";

            return statusLine;
        }

        public void Purge()
        {
            const string sqlCmd = "DELETE [Expenses]";

            using (ExpenseDBDataContext dc = new ExpenseDBDataContext())
            {
                dc.ExecuteCommand(sqlCmd);
            }
        }
    }
}


*)