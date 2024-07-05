using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomPropertyDrawer(typeof(AbstractClassAttribute))]
public class AbstractClassDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AbstractClassAttribute attribute = (AbstractClassAttribute)base.attribute;

        if (attribute.baseType == null || !attribute.baseType.IsClass || attribute.baseType.IsSealed)
        {
            EditorGUI.LabelField(position, label.text, "Invalid type");
            return;
        }

        // Get all classes that inherit from the specified base type
        List<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => attribute.baseType.IsAssignableFrom(x) && !x.IsAbstract && x.IsClass)
            .ToList();

        // Get class names
        List<string> classNames = allTypes.Select(x => x.FullName).ToList();

        // Display as dropdown list
        int selectedIndex = classNames.IndexOf(property.stringValue);
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, classNames.ToArray());

        // Update selected value
        if (selectedIndex >= 0 && selectedIndex < classNames.Count)
        {
            property.stringValue = classNames[selectedIndex];
        }
        else
        {
            property.stringValue = "";
        }
    }
}