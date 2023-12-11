using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SQLData.Utilities.Command;
using SQLData.Model;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using SQLData.Applications.RevitCommand;
using System.Windows.Forms;

namespace SQLData.ViewModel
{
    public class SQLDataViewModel : INotifyPropertyChanged
    {
        Document document = Applications.RevitCommand.SQLData.document;

        SQLDatabaseConnect sqlConnection = new SQLDatabaseConnect();

        #region Constructor 

        public SQLDataViewModel()
        {
            sqlConnection.ConnectDataBase();
            CreateSQLTableCommand = new MyCommand(CreateSQLTableCommandExcute, CreateSQLTableCommandCanExcute);
            ExportDataCommand = new MyCommand(ExportDataCommandExcute, ExportDataCommandCanExcute);
            ImportDataCommand = new MyCommand(ImportDataCommandExcute, ImportDataCommandCanExcute);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChange([CallerMemberName] string propertyName = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region public properties 

        public MyCommand CreateSQLTableCommand { get; set; }
        public MyCommand ExportDataCommand { get; set; }
        public MyCommand ImportDataCommand { get; set; }

        #endregion

        #region public methods 


        public void CreateSQLTableCommandExcute(object parameter)
        {
            bool doesExist = TableExists("RevitData", "Doors");

            if(doesExist)
            {
                TaskDialog.Show("SQL Table error", "Table already exists"); 
            }
            else
            {
                try
                {
                    string tableQuery = "CREATE TABLE Doors" +
                        "(UniqueId varchar(255) NOT NULL PRIMARY KEY, FamilyType varchar(255), " +
                        "Mark varchar(255), DoorFinish varchar(255))";

                    SqlCommand command = sqlConnection.Query(tableQuery);
                    command.ExecuteNonQuery();

                    TaskDialog.Show("Create SQL Table", "Doors table created");
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("SQL Error", ex.ToString());                    
                }
            }

        }
                   

        public void ExportDataCommandExcute(object parameter)
        {
            IList<Element> doors = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().ToElements();


            string setQuery = "INSERT INTO Doors(UniqueId, FamilyType, Mark, DoorFinish) " +
                "VALUES(@param1, @param2, @param3, @param4)";

            foreach (Element element in doors)
            {
                Parameter doorFinish = element.get_Parameter(BuiltInParameter.DOOR_FINISH);
                string dFinish;

                if(doorFinish.HasValue == true)
                {
                    dFinish = doorFinish.AsString();
                }
                else
                {
                    dFinish = "";
                }

                Parameter doorMark = element.LookupParameter("Mark");
                string dMark = doorMark.AsString();


                using(SqlCommand command = sqlConnection.Query(setQuery))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@param1", element.UniqueId);
                        command.Parameters.AddWithValue("@param2", element.Name);
                        command.Parameters.AddWithValue("@param3", dMark);
                        command.Parameters.AddWithValue("@param4", dFinish);
                        command.ExecuteNonQuery();
                    }
                    catch(Exception ex) 
                    {
                        TaskDialog.Show("SQL Insert Error", ex.ToString());
                    }
                }
            }

            TaskDialog.Show("Door Values Export", "Door Values added!");
        }


        public void ImportDataCommandExcute(object parameter)
        {

            string getQuery = "SELECT * FROM Doors";

            SqlCommand command = sqlConnection.Query(getQuery); 
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string uID = reader["UniqueId"].ToString();
                string dfinish = reader["DoorFinish"].ToString();

                Element element = document.GetElement(uID);
                Parameter doorFinish = element.get_Parameter(BuiltInParameter.DOOR_FINISH);

                using(Transaction transaction = new Transaction(document, "Set Data"))
                {
                    transaction.Start();

                    doorFinish.Set(dfinish);

                    transaction.Commit();   
                }
            }
            reader.Close();

            TaskDialog.Show("Update Door Finish Values", "Doors values updated");
            
        }


        public bool CreateSQLTableCommandCanExcute(object parameter)
        {
            return true;
        }

        public bool ExportDataCommandCanExcute(object parameter)
        {
            return true;
        }

        public bool ImportDataCommandCanExcute(object parameter)
        {
            return true;
        }
        #endregion


        #region Private Methods

        private bool TableExists (string database, string name)
        {
            try
            {
                string existsQuery = "select case when exists((select * FROM  [" + database + "].sys.tables " + "WHERE name = '" + name + "')) then 1 else 0 end";

                SqlCommand command = sqlConnection.Query(existsQuery);
                return (int)command.ExecuteScalar() == 1;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.ToString());
                return true;
            }
                
        }

        #endregion

    }
}
