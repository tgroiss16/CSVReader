using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Try
{
    class Program
    {
        static void Main(string[] args)
        {
            List<BusinessCard> comp = ReadPythonCSV();
            comp.ForEach(x => Console.WriteLine(x.CompanyName));
        }

        private static String[] countrylist = new String[200];

        public static List<BusinessCard> ReadPythonCSV()
        {
            //contactName, companyName, telefon, fax, email, city, postalCode, street, country, notes
            List<BusinessCard> listofCompanies = new List<BusinessCard>();
            String filePath = "pythonCSV.csv";
            String[] sumOfLines = File.ReadAllLines(filePath);
            LoadCountrylist();
            String contactName = "";
            String firstname = "";
            String lastname = "";
            String companyName = "";
            String phoneNumberMobile = "";
            String fax = "";
            String email = "";

            String city = "";
            String postalCode = "";
            String street = "";
            String country = "";
            String notes = "";


            foreach (String line in sumOfLines)
            {
                String lineRef = CheckForUmlauts(line);


                if (lineRef.Contains("Firma:"))
                {
                    companyName = lineRef.Substring(7);
                }
                else if (lineRef.Contains("Name:"))
                {
                    contactName = lineRef.Substring(5);
                    String[] names = contactName.Split(' ').Where(x => !x.Equals("")).ToArray();
                    if (names.Length == 1)
                    {
                        notes += "Name = " + contactName;
                    }
                    else if (names.Length == 2)
                    {
                        firstname = names[0];
                        lastname = names[1];
                    }
                    else if(names.Length >= 3)
                    {
                        notes += "Name = "+contactName;
                    }
                }
                else if (lineRef.Contains("Fax:"))
                {
                    fax = lineRef.Substring(4);
                }
                else if(lineRef.Contains("Telefon (berufl.):"))
                {
                    phoneNumberMobile = lineRef.Substring(18);
                }
                else if (lineRef.Contains("email:"))
                {
                    email = lineRef.Substring(6);
                }
                else if (lineRef.Contains("Adresse:"))
                {
                    //Street - Postalcode - City - Country - Else
                    String[] add = SplitAdresse(lineRef.Substring(9));
                    //Man bekommt die geteilte Adresse so zurück wie drüber beschrieben in dem Array. Sollte in Else was drin sein wurde es nicht richtig geparst
                    if (add[4].Length > 2)
                    {
                        notes += "Adresse: " + add[4];
                    }
                    else
                    {
                        street = add[0];
                        postalCode = add[1];
                        city = add[2];
                        country = add[3];
                    }

                   
                    
                }
                else if (lineRef.Contains("EndLine---"))
                {
                    BusinessCard comp = new BusinessCard()
                    {
                        ContactFirstName = firstname,
                        ContactLastName = lastname,                        
                        CompanyName = companyName,
                        Telefon = phoneNumberMobile,
                        Fax = fax,
                        Email = email,
                        City = city,
                        Street = street,
                        PostalCode = postalCode,
                        Country = country,
                        Notes = notes 
                    };
                    listofCompanies.Add(comp);
                    contactName = "";
                    firstname = "";
                    lastname = "";
                    companyName = "";
                    phoneNumberMobile = "";
                    fax = "";
                    email = "";

                    city = "";
                    postalCode = "";
                    street = "";
                    country = "";
                    notes = "";
                }
            }

            return listofCompanies;
        }

        private static void LoadCountrylist()
        {
            String filePath = "country-keyword-list.csv";
            countrylist = File.ReadAllLines(filePath);
        }

        private static String[] SplitAdresse(String fulladresse)
        {
            String[] split = fulladresse.Split(' ').Where(x => !x.Equals("")).ToArray();
            String[] solution = new String[5];
            //Street - Postalcode - City - Country - Else
            String linebefore = "";
            bool catchnext = false;
            bool successPostal = false;
            bool successStreet = false;
            foreach (String line in split)
            {
                String postal = IsPostalCode(line);
                int count = countrylist.Where(x => x.Contains(line)).Count();
                //After the Postalcode its nearly always the city
                if (catchnext)
                {
                    solution[2] = line;
                    catchnext = false;
                    successPostal = true;
                }
                //Postalcode?
                else if (!postal.Equals("no"))
                {
                    solution[1] = postal;
                    catchnext = true;
                }
                //Country, vergleichts mit der CSV Liste von allen Countries
                else if (count > 0)
                {
                    solution[3] = line;
                    count = 0;
                }
                //Die Hausnummer ist meistens nach der Street
                else if (line.Length <= 3 && IsDigitsOnly(line))
                {
                    solution[0] = linebefore + " " + line;
                    successStreet = true;
                }


                linebefore = line;
            }

            if (successStreet && successPostal)
            {
                solution[4] = " ";
                return solution;
            }
            else{
                solution[4] = fulladresse;
                return solution;
            }
        }

        private static String IsPostalCode(string line)
        {
            String solution = "no";
            String[] testpostal = new String[2];
            testpostal = line.Split('-');
            if (IsDigitsOnly(line))
            {
                if (line.Length == 4 || line.Length == 5)
                {
                    solution = line;
                }
            }

            if (testpostal.Length > 1) 
            {
                if (IsDigitsOnly(testpostal[1]))
                {
                    if (testpostal[1].Length == 4 || testpostal[1].Length == 5)
                    {
                        solution = testpostal[1];
                    }
                }
            }

            return solution;
        }

        private static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private static String CheckForUmlauts(String umlaut)
        {
            if (umlaut.Contains("=FC"))
            {
               umlaut = umlaut.Replace("=FC", "ü");
            }
            if (umlaut.Contains("=DF"))
            {
                umlaut = umlaut.Replace("=DF", "ß");
            }
            if (umlaut.Contains("=F6"))
            {
                umlaut = umlaut.Replace("=F6", "Ö");
            }
            if (umlaut.Contains("=B0"))
            {
                umlaut = umlaut.Replace("=B0", "°");
            }
            if (umlaut.Contains("=E4"))
            {
                umlaut = umlaut.Replace("=E4", "ä");
            }
            
            //if (umlaut.Contains(""))
            //{
            //    umlaut = umlaut.Replace("", "");
            //}

            return umlaut;
        }
    }
}
