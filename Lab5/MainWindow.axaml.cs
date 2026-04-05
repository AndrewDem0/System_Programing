using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicTypeIdentification
{
    public class Doctor
    {
        public string FullName { get; set; }
        public int ExperienceYears { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> Specialties { get; set; } 

        public Doctor()
        {
            FullName = "Невідомо";
            ExperienceYears = 0;
            IsAvailable = false;
            Specialties = new List<string>();
        }

        public Doctor(string fullName, int experienceYears, bool isAvailable, List<string> specialties)
        {
            FullName = fullName;
            ExperienceYears = experienceYears;
            IsAvailable = isAvailable;
            Specialties = specialties;
        }

        public void AddSpecialty(string specialty)
        {
            Specialties.Add(specialty);
        }

        public string DiagnosePatient(string patientName)
        {
            return $"Лікар {FullName} проводить огляд пацієнта: {patientName}.";
        }

        public void ToggleAvailability()
        {
            IsAvailable = !IsAvailable;
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnReflect_Click(object sender, RoutedEventArgs e)
        {
            Doctor targetDoctor = new Doctor(
                "Грегорі Хаус", 
                25, 
                true, 
                new List<string> { "Інфекціоніст", "Нефролог", "Діагност" }
            );

            Type targetType = targetDoctor.GetType();
            List<TreeViewItem> globalRootNodes = new List<TreeViewItem>();

            TreeViewItem ctorsRoot = new TreeViewItem { Header = "[ КОНСТРУКТОРИ ]", IsExpanded = true };
            List<TreeViewItem> ctorsList = new List<TreeViewItem>();
            
            ConstructorInfo[] constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (ConstructorInfo ctor in constructors)
            {
                // Форматування списку параметрів за допомогою LINQ
                string paramSignature = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                ctorsList.Add(new TreeViewItem { Header = $"ctor Doctor({paramSignature})" });
            }
            ctorsRoot.ItemsSource = ctorsList;
            globalRootNodes.Add(ctorsRoot);

            TreeViewItem propsRoot = new TreeViewItem { Header = "[ ВЛАСТИВОСТІ ТА СТАН ОБ'ЄКТА ]", IsExpanded = true };
            List<TreeViewItem> propsList = new List<TreeViewItem>();

            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in properties)
            {
                object value = prop.GetValue(targetDoctor);
                string headerData = $"[Властивість] {prop.PropertyType.Name} {prop.Name} = ";
                TreeViewItem node = new TreeViewItem();

                if (value is IEnumerable collection && value is not string)
                {
                    node.Header = headerData + "{Колекція}";
                    List<TreeViewItem> childNodes = new List<TreeViewItem>();
                    foreach (object element in collection)
                    {
                        childNodes.Add(new TreeViewItem { Header = element.ToString() });
                    }
                    node.ItemsSource = childNodes;
                }
                else
                {
                    node.Header = headerData + (value?.ToString() ?? "null");
                }
                propsList.Add(node);
            }
            propsRoot.ItemsSource = propsList;
            globalRootNodes.Add(propsRoot);

            TreeViewItem methodsRoot = new TreeViewItem { Header = "[ МЕТОДИ ]", IsExpanded = true };
            List<TreeViewItem> methodsList = new List<TreeViewItem>();

            // BindingFlags.DeclaredOnly ігнорує методи класу Object
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo method in methods)
            {
                // Відсікання методів get_ та set_ , які генеруються для властивостей
                if (method.IsSpecialName) continue;

                string paramSignature = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                methodsList.Add(new TreeViewItem { Header = $"[Метод] {method.ReturnType.Name} {method.Name}({paramSignature})" });
            }
            methodsRoot.ItemsSource = methodsList;
            globalRootNodes.Add(methodsRoot);

            PropertiesTreeView.ItemsSource = globalRootNodes;
            BtnReflect.IsEnabled = false;
        }
    }
}