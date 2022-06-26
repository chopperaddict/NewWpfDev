using NewWpfDev.ViewModels;
using NewWpfDev. Views;

using System;
using System . Collections . Generic;
using System . ComponentModel;
using System . Linq;
using System . Net;
using System . Text;
using System . Threading . Tasks;
using System . Windows . Input;
using System . Xml . Linq;

namespace NewWpfDev. Models
{
    public class Person :  IDataErrorInfo
    {
        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged ( string propertyName )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged ( this , new PropertyChangedEventArgs ( propertyName ) );
            }
        }
        #endregion NotifyPropertyChanged

        #region Properties
        private string name;
        private string address;
        private int age;
        private Person selectedperson;
        private string errorInfo1;
        private string errorInfo2;
        private string invalidchars = "0123456789!£$%^&*()}{@\"~#?//>\"\"<,.\\¬";

        public int Age
        {
            get { return age; }
            set { age = value; NotifyPropertyChanged ( "Age" ); }
        }
        public string Name
        {
            get
            { return name; }
            set { name = value; NotifyPropertyChanged ( "Name" ); }
        }
        public string Address
        {
            get
            { return address; }
            set { address = value; NotifyPropertyChanged ( "Address" ); }
        }
        public Person SelectedPerson
        {
            get { return selectedperson; }
            set { selectedperson = value; NotifyPropertyChanged ( "SelectedPerson" ); }
        }
        public string ErrorInfo1
        {
            get { return errorInfo1; }
            set { errorInfo1 = value; NotifyPropertyChanged ( "ErrorInfo1" ); }
        }
        public string ErrorInfo2
        {
            get { return errorInfo2; }
            set { errorInfo2 = value; NotifyPropertyChanged ( "ErrorInfo2" ); }
        }

        public string Error { get { return "An Error has occured in the data entered.."; } }

        public string this [ string Propertyname ]
        {
            // Validate a name to be exactly 2 words in length
            // with only Alpha characters
            // Reports back if any other values are identified
            // Responses are targetted at a Full Property (ErrorInfo x ) Text field in the caller module
            // This does NOT provide ToolTip response
            get
            {
                string result = null;
                int reslt = -1;
                string ch = "";
                if ( Propertyname == "Name" )
                {
                    if ( Name == null )
                        return null;
                    reslt = CheckforValidChars ( this . Name , out ch );
                    if ( reslt != -1 )
                    {
                        this . ErrorInfo1 = $"Entry contains an invalid character of '{ch}' at position {reslt}...";
                        return ErrorInfo1;
                    }
                    string [ ] clauses = this . Name . Split ( ' ' );
                    if ( clauses . Length >= 3 )
                    {
                        this . ErrorInfo1 = "User Name can ONLY be 2 words with >= 1 space between them";
                        return ErrorInfo1;
                    }
                    // Works well
                    if ( this . Name == null || string . IsNullOrWhiteSpace ( this . Name ) )
                    {
                        this . ErrorInfo1 = "Please enter User Name as 2 words with >= 1 space between them";
                        this . ErrorInfo2 = "Please enter a valid Age between 16 & 109 ....";
                        return ErrorInfo1;
                    }
                    else if ( this . Name . Contains ( " " ) == false )
                    {
                        this . ErrorInfo1 = "User Name must be at least 2 seperate words....";
                        return ErrorInfo1;
                    }
                    else if ( this . Name . Length > 6 )
                    {
                        string [ ] str = this . Name . Split ( ' ' );
                        if ( str . Length >= 2 )
                            if ( str [ 1 ] != "" && str [ 1 ] . Length < 2 )
                                this . ErrorInfo1 = "User Name must be two seperate words each > 1 in length....";
                            else
                                this . ErrorInfo1 = "";
                        return ErrorInfo1;
                    }
                    else
                    {
                        this . ErrorInfo1 = "The User Name must be 2 words totalling at least 6 characters overall....";
                        return ErrorInfo1;
                    }
                }
                else if ( Propertyname == "Age" )
                {
                    if ( this . Age <= 15 || this . Age > 110 )
                    {
                        this . ErrorInfo2 = "Please enter a valid Age between 16 & 109 ....";
                        result = "A valid age > 16 & < 110 must be entered. !!!";
                    }
                    else
                    {
                        this . ErrorInfo2 = "Age is Valid....";
                        return "Age is Valid....";
                    }
                }
                return result;
            }
        }
        private int CheckforValidChars ( string entry , out string ch )
        {
            ch = "";
            bool success = true;
            int result = -1;
            if ( entry == null )
                return -1;
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            try
            {
                foreach ( var item in invalidchars )
                {
                    if ( entry . Contains ( item ) )
                    {
                        ch = entry [ result ] . ToString ( );
                        success = false;
                        for ( int y = 0 ; y < entry . Length ; y++ )
                        {
                            if ( entry [ y ] == item )
                            {
                                result = y;
                                ch = item . ToString ( );
                                break;
                            }
                        }
                        break;
                    }
                    result++;
                }
            }
            catch (Exception ex) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            if ( !success )
                return result;
            else return -1;
        }
    #endregion Properties
    public Person ( )
        {
        }

    }

}