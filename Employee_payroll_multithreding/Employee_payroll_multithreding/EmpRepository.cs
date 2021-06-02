using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Employee_payroll_multithreding
{
    public class EmpRepository
    {
        //Connection string
        public static string connectionString = @"Data Source=DESKTOP-DL043RM;Initial Catalog=payroll;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; //Specifying the connection string from the sql server connection.
        // Establishing the connection using the Sqlconnection. 
        SqlConnection connection = new SqlConnection(connectionString);
        public List<EmployeeModel> employeeList;

        public bool DataBaseConnection()//Adding method boolean type which return true if there is connection
        {
            try
            {
                //create object DateTime class
                //DateTime.Now class access system date and time 
                //now means what will the date and time on your system it will display 
                DateTime now = DateTime.Now;
                // open connection
                connection.Open();
                //using SqlConnection
                using (connection)  
                {
                    Console.WriteLine($"Connection is created Successful {now}"); 

                }
                //close connection
                connection.Close(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }
        // UC1:- Ability to add multiple employee to payroll using the payroll database created 

        public bool AddEmployeeListToDataBase(List<EmployeeModel> employeeList)
        {
            foreach (var employee in employeeList)
            {
                Console.WriteLine("Employeee being added :", employee.EmployeeName);
                //Bool flag for AddEmployee
                bool flag = AddEmployeeToDataBase(employee);
                Console.WriteLine("Employee Added :", employee.EmployeeName);
                //If the not added employee name to the employee then return false
                if (flag == false)
                    return false;
            }
            //if there is value available then return true and executive
            return true;
        }
        //Add employee to the database
        public bool AddEmployeeToDataBase(EmployeeModel model)
        {
            try
            {
                using (connection)
                {
                    //Creating a stored Procedure for adding employees into database
                    //New instance of sqlcommand
                    //SQLCommand is use for transact query and stored procedure to excute the query
                    SqlCommand command = new SqlCommand("dbo.spInsertData", this.connection); 

                    command.CommandType = CommandType.StoredProcedure; //Command type is a class to set as stored procedure
                                                                       // Adding values from employeemodel to stored procedure 

                    command.Parameters.AddWithValue("@EmployeeName", model.EmployeeName);
                    command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    command.Parameters.AddWithValue("@Address", model.Address);
                    command.Parameters.AddWithValue("@Department", model.Department);
                    command.Parameters.AddWithValue("@Gender", model.Gender);
                    command.Parameters.AddWithValue("@BasicPay", model.BasicPay);
                    command.Parameters.AddWithValue("@Deductions", model.Deductions);
                    command.Parameters.AddWithValue("@TaxablePay", model.TaxablePay);
                    command.Parameters.AddWithValue("@Tax", model.Tax);
                    command.Parameters.AddWithValue("@NetPay", model.NetPay);
                    command.Parameters.AddWithValue("@StartDate", model.StartDate);
                    command.Parameters.AddWithValue("@City", model.City);
                    command.Parameters.AddWithValue("@Country", model.Country);
                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    connection.Close();

                    if (result != 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                connection.Close();
            }

        }
        // UC2:- Ability to add multiple employee to payroll DB using Threads so as to get a better response

        public void AddEmployeeDataBaseWithThread(List<EmployeeModel> employeelList)
        {
            //For each employeeData present in list new thread
            //is created and all threads run according to the time slot assigned by the thread scheduler.
            //It reprsents all data form the list
            employeelList.ForEach(employeeData =>
            {
                //The Task class represents a single operation that does not return a value and that usually executes asynchronously.
                //Asynchronous programming is an efficient approach towards activities blocked or access is delayed.
                Task thread = new Task(() =>
                {
                    // Printing the current thread id being utilised
                    Console.WriteLine("Employee Being added" + employeeData.EmployeeName);
                    // Calling the method to add the data to the address book database
                    //Thread Create and control thread,sets its priority,And get its status
                    //Current thread get currently running state
                    //There is Id for Thread Manageing which manage the thread 
                    Console.WriteLine("Current thread id: " + Thread.CurrentThread.ManagedThreadId); 
                    this.AddEmployeeToDataBase(employeeData);
                    // Indicating mesasage to end of data 
                    Console.WriteLine("Employee added:" + employeeData.EmployeeName); 
                });
                //For the Task Start Thread
                thread.Start();
            });
        }

    }
}
    