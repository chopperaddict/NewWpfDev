using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;
using System . Windows;
using System . Windows . Controls;
using System . Windows . Data;
using System . Windows . Documents;
using System . Windows . Input;
using System . Windows . Media;
using System . Windows . Shapes;

using NewWpfDev. Models;
using NewWpfDev. UserControls;

using static NewWpfDev. UserControls . TextBoxwithDataError;

namespace NewWpfDev. Views
{
    /// <summary>
    /// Interaction logic for DataErrorWindow.xaml
    /// Which hosts both a direct DataErrorinfo supported TextBox
    /// and a usercontrol that does the same thing itself
    /// </summary>
    public partial class DataErrorWindow : Window
    {
        //public customMessageHandler msghandler ;
        ValidateUsernameClass tclass = new ValidateUsernameClass ( );
        ValidateUsernameClass tclass2 = new ValidateUsernameClass ( );
        ValidateUsernameClass tclass3 = new ValidateUsernameClass ( );
        ValidateCounty countyclass = new ValidateCounty ( );
        UserControl uc = new UserControl ( );
        Person person = new Person ( );
        public DataErrorWindow ( )
        {
            InitializeComponent ( );
            // Now setup the ViewModel it will use
            //InitViewModel ( tclass );
            ////Include the "Global) Usercontrol that uses the viewmodel above
            //uc = AddUserControl ( );
            ////Add the Usercontrol itself
            //TextBoxwithDataError tbw = AddValidatedTexBox ( tclass );
            
            
            secondtextbox . SetDc ( tclass2 );
            secondtextbox . Height = 65;
            tclass2 . PromptFontsize = 16;
            tclass2 . PromptText = "Enter User Name :-";
            // Doesnt work !!!
            //tclass . PromptBkground = FindResource ( "Gray0" ) as Brush;
            // show or hide prompt bar above textbox
            tclass2 . IsPromptVisible = Visibility . Visible;
            //Configure date entry field
            tclass2 . TextboxBkground = FindResource ( "Yellow3") as  SolidColorBrush;
            tclass2 . TextboxFground = FindResource ( "Black0" ) as SolidColorBrush;
            tclass2 . DataFontSize = 18;
            //Setup a RoutedEvent to pass data to anyone listening for it when Enter is hit or it looses focus (if Valid)
            secondtextbox. SendUser += new SendUserHandler ( UpdateUserNameEventHandler2 );


            thirdtextbox . SetDc ( tclass3 );
            tclass3 . PromptFontsize = 16;
            tclass3 . PromptText = "Enter Another username";
            // Doesnt work !!!
            //tclass . PromptBkground = FindResource ( "Gray0" ) as Brush;
            // show or hide prompt bar above textbox
            tclass3 . IsPromptVisible = Visibility . Visible;
            //Configure date entry field
            tclass3 . TextboxBkground = FindResource ( "Blue4" ) as SolidColorBrush;
            tclass3 . TextboxFground = FindResource ( "White0" ) as SolidColorBrush;
            tclass3 . DataFontSize = 18;
            thirdtextbox . SendUser += new SendUserHandler ( UpdateUserNameEventHandler3 );

            countytextbox. SetDc ( countyclass);
            countyclass. PromptFontsize = 20;
            countyclass . PromptText = "Enter County Name";
            // Doesnt work !!!
            //tclass . PromptBkground = FindResource ( "Gray0" ) as Brush;
            // show or hide prompt bar above textbox
            countyclass . IsPromptVisible = Visibility . Visible;
            //Configure date entry field
            countyclass . TextboxBkground = FindResource ( "Gray5" ) as SolidColorBrush;
            countyclass . TextboxFground = FindResource ( "Black0" ) as SolidColorBrush;
            countyclass . DataFontSize = 20;
            countytextbox . SendUser += new SendUserHandler ( UpdateCountyEventHandler );

            Closebtn . DataContext = tclass;
        }
        private UserControl AddUserControl ( )
        {
            // Add Usercontrol to screen
            this . DataContext = person;
            uc . Visibility = Visibility . Visible;
            uc . Height = 50;
            uc . Width = 300;
            uc . HorizontalAlignment = HorizontalAlignment . Left;
            spanel . Children . Add ( uc );
            return uc;
        }
        private TextBoxwithDataError AddValidatedTexBox ( ValidateUsernameClass tclass )
        {
            // setup the Usercontrols (ValidatedTextbox) content
            TextBoxwithDataError tbw = new TextBoxwithDataError ( );
            Thickness th = new Thickness ( );
            th . Left = 0;
            th . Top = 0;
            th . Bottom = 0;
            th . Right = 0;
            tbw . BorderThickness = th;
            //tbw . BorderBrush = FindResource ( "Red4" ) as SolidColorBrush;
            uc . Content = tbw;
            // Configure the Control to its VewModel
            tbw . SetDc ( tclass );
            
            //Setup a RoutedEvent to pass data to anyone listening for it when Enter is hit or it looses focus (if Valid)
            tbw . SendUser += new SendUserHandler ( UpdateUserNameEventHandler2);
            return tbw;
        }
        private void InitViewModel ( ValidateUsernameClass tclass )
        {
            // Now setup the ViewModel it will use
            // Configure prompt
            tclass . PromptFontsize = 16;
            tclass . PromptText = "Enter Main User Name :-";
            // Doesnt work !!!
            //tclass . PromptBkground = FindResource ( "Gray0" ) as Brush;
            // show or hide prompt bar above textbox
            tclass . IsPromptVisible = Visibility . Visible;
            //Configure date entry field
            tclass . TextboxBkground = FindResource ( "Gray3" ) as SolidColorBrush;
            tclass . TextboxFground = FindResource ( "White0" ) as SolidColorBrush;
            tclass . DataFontSize = 18;
        }
        protected void UpdateUserNameEventHandler2 ( object sender , MessageEventArgs e )
        {
            //Handle event sent from TextBoxwithDataError user control on Focus lost or Enter hit
            string str = e . message;
            // Check that the data is actually valid from ViewModel flag IsValid
            if ( tclass2 . IsValid )
            {
                person . Name = str;
                Debug. WriteLine ( $"Data below from user control is valid\n{tclass2 . UserName}" );
                result1 . Text = str;
            }
            else
            {
                Debug. WriteLine ( $"Data in user control is Invalid\n{tclass2 . UserName}" );
                MessageBox . Show ( "The data entered is NOT valid !!" , "Error" );
            }
        }
        protected void UpdateUserNameEventHandler3 ( object sender , MessageEventArgs e )
        {
            //Handle event sent from TextBoxwithDataError user control on Focus lost or Enter hit
            string str = e . message;
            // Check that the data is actually valid from ViewModel flag IsValid
            if ( tclass3 . IsValid )
            {
                person . Name = str;
                Debug. WriteLine ( $"Data below from user control is valid\n{tclass3 . UserName}" );
                result2 . Text = str;
            }
            else
            {
                Debug. WriteLine ( $"Data in user control is Invalid\n{tclass3 . UserName}" );
                MessageBox . Show ( "The data entered is NOT valid !!" , "Error" );
            }
        }
        protected void UpdateCountyEventHandler ( object sender , MessageEventArgs e )
        {
            //Handle event sent from TextBoxwithDataError user control on Focus lost or Enter hit
            string str = e . message;
            // Check that the data is actually valid from ViewModel flag IsValid
            if ( countyclass. IsValid )
            {
                person . Name = str;
                 $"Data below from user control is valid\n{countyclass . County}".cwerror();
                result3 . Text = str;
            }
            else
            {
                Debug. WriteLine ( $"Data in user control is Invalid\n{tclass3 . UserName}" );
                MessageBox . Show ( "The data entered is NOT valid !!" , "Error" );
            }
        }
        private void Window_Closing ( object sender , System . ComponentModel . CancelEventArgs e )
        {
            // cleanup in case we reopen it in thsame session.
            this . DataContext = null;
        }

        private void CloseBtn ( object sender , RoutedEventArgs e )
        {
            this . Close ( );
        }
    }
}
