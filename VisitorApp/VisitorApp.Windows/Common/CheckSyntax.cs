using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace VisitorApp
{
    public class CheckSyntax
    {
        //Maintaining Syntax order around Controls

        public static void checkOnlyName(object sender, KeyRoutedEventArgs e)
        {
            //For Name, Surname, Etc

            var checkTxtText = sender as TextBox;
            int txtlength = checkTxtText.Text.Length;
            string txt = checkTxtText.Text;
            TextBox txtt = new TextBox();
            for (int i = 0; i < txtlength; i++)
            {
                if (!char.IsLetter(txt[i]) && txt[i].ToString() != " " && txt[i].ToString() != "-")
                {
                    //Do nothing; Basically skip the text then add the rest
                }
                else
                {
                    txtt.Text = txtt.Text + txt[i].ToString();
                }
                checkTxtText.Text = txtt.Text;
                checkTxtText.SelectionStart = checkTxtText.Text.Length;
            }
        }

        public static void checkOnlyNumber(object sender, KeyRoutedEventArgs e)
        {
            //For Phone Number

            var checkTxtNumber = sender as TextBox;
            int txtlength = checkTxtNumber.Text.Length;
            string txt = checkTxtNumber.Text;
            TextBox txtt = new TextBox();
            for (int i = 0; i < txtlength; i++)
            {
                if (!char.IsNumber(txt[i]))
                {
                    //Do nothing; Basically skip the text then add the rest
                }
                else
                {
                    txtt.Text = txtt.Text + txt[i].ToString();
                }
                checkTxtNumber.Text = txtt.Text;
                checkTxtNumber.SelectionStart = checkTxtNumber.Text.Length;
            }
        }

        public  void checkEmail(object sender, KeyRoutedEventArgs e)
        {
            
        }
    }
}
