using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace DynamicTypeIdentification
{
    // Цільовий клас для інспектування
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

    // Модель даних для MVVM-прив'язки
    public class ReflectionNode
    {
        public string Header { get; set; }
        public ObservableCollection<ReflectionNode> Children { get; set; }

        public ReflectionNode(string header)
        {
            Header = header;
            Children = new ObservableCollection<ReflectionNode>();
        }
    }

    // Ізольований сервіс для аналізу метаданих
    public class TypeInspectorService
    {
        public ObservableCollection<ReflectionNode> Inspect(object targetObject)
        {
            var rootNodes = new ObservableCollection<ReflectionNode>();
            if (targetObject == null) return rootNodes;

            Type targetType = targetObject.GetType();

            // 1. Аналіз конструкторів
            var ctorsRoot = new ReflectionNode("[ КОНСТРУКТОРИ ]");
            ConstructorInfo[] constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (ConstructorInfo ctor in constructors)
            {
                string paramSignature = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                ctorsRoot.Children.Add(new ReflectionNode($"ctor {targetType.Name}({paramSignature})"));
            }
            rootNodes.Add(ctorsRoot);

            // 2. Аналіз властивостей та поточного стану
            var propsRoot = new ReflectionNode("[ ВЛАСТИВОСТІ ТА СТАН ОБ'ЄКТА ]");
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in properties)
            {
                object value = null;
                try
                {
                    value = prop.GetValue(targetObject);
                }
                catch (TargetParameterCountException) 
                {
                    value = "{Потребує індексу}";
                }

                string headerData = $"[Властивість] {prop.PropertyType.Name} {prop.Name} = ";
                ReflectionNode node = new ReflectionNode(headerData);

                if (value is IEnumerable collection && value is not string)
                {
                    node.Header = headerData + "{Колекція}";
                    foreach (object element in collection)
                    {
                        node.Children.Add(new ReflectionNode(element?.ToString() ?? "null"));
                    }
                }
                else
                {
                    node.Header = headerData + (value?.ToString() ?? "null");
                }
                propsRoot.Children.Add(node);
            }
            rootNodes.Add(propsRoot);

            // 3. Аналіз методів
            var methodsRoot = new ReflectionNode("[ МЕТОДИ ]");
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo method in methods)
            {
                if (method.IsSpecialName) continue;

                string paramSignature = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                methodsRoot.Children.Add(new ReflectionNode($"[Метод] {method.ReturnType.Name} {method.Name}({paramSignature})"));
            }
            rootNodes.Add(methodsRoot);

            return rootNodes;
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
            // Ініціалізація екземпляра як System.Object для підтвердження пізнього зв'язування
            object unknownTarget = new Doctor(
                "Грегорі Хаус", 
                25, 
                true, 
                new List<string> { "Інфекціоніст", "Нефролог", "Діагност" }
            );

            // Виклик сервісу інтроспекції
            var inspector = new TypeInspectorService();
            ObservableCollection<ReflectionNode> inspectionData = inspector.Inspect(unknownTarget);

            // Прив'язка даних до інтерфейсу
            PropertiesTreeView.ItemsSource = inspectionData;
            BtnReflect.IsEnabled = false;
        }
    }
}