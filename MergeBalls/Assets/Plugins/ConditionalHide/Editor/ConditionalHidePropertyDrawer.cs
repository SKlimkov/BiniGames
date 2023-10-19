using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System.Reflection;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: -

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    private static string s_InvalidTypeMessage = L10n.Tr("Use Range with float or int.");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Debug.LogErrorFormat("OnGUI {0}, {1}", property.name, property.propertyPath);
        var condHAtt = (ConditionalHideAttribute)attribute;
        var enabled = GetConditionalHideAttributeResult(condHAtt, property);

        var wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            if (condHAtt.WithRange) {
                if (property.propertyType == SerializedPropertyType.Float)
                    EditorGUI.Slider(position, property, condHAtt.Min, condHAtt.Max, label);
                else if (property.propertyType == SerializedPropertyType.Integer)
                    EditorGUI.IntSlider(position, property, (int)condHAtt.Min, (int)condHAtt.Max, label);
                else
                    EditorGUI.LabelField(position, label.text, s_InvalidTypeMessage);
            }
            else {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }        

        GUI.enabled = wasEnabled;
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property) {
        ConditionalHideAttribute range = (ConditionalHideAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.Float) {
            var slider = new Slider(property.displayName, range.Min, range.Max);
            slider.bindingPath = property.propertyPath;
            slider.showInputField = true;
            return slider;
        }
        else if (property.propertyType == SerializedPropertyType.Integer) {
            var intSlider = new SliderInt(property.displayName, (int)range.Min, (int)range.Max);
            intSlider.bindingPath = property.propertyPath;
            intSlider.showInputField = true;
            return intSlider;
        }

        return new Label(s_InvalidTypeMessage);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            //The property is not being drawn
            //We want to undo the spacing added before and after the property
            return -EditorGUIUtility.standardVerticalSpacing;
            //return 0.0f;
        }
    }

    public static  bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = (condHAtt.UseOrLogic) ?false :true;

        var propertyPath = property.propertyPath;
        var conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
        var sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue == null) {
            //original implementation (doens't work with nested serializedObjects)
            sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField);
        }

        if (sourcePropertyValue != null)
        {
            enabled = CheckPropertyType(sourcePropertyValue, condHAtt.ConditionalEnumValue);
            if (condHAtt.InverseCondition1) enabled = !enabled;             
        }
        else
        {
            //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField2);
        var sourcePropertyValue2 = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue2 == null) {
            //original implementation (doens't work with nested serializedObjects)
            sourcePropertyValue2 = property.serializedObject.FindProperty(condHAtt.ConditionalSourceField2);
        }

        if (sourcePropertyValue2 != null)
        {
            Debug.LogErrorFormat("ACHTUNG!");
            bool prop2Enabled = CheckPropertyType(sourcePropertyValue2, condHAtt.ConditionalEnumValue);
            if (condHAtt.InverseCondition2) prop2Enabled = !prop2Enabled;

            if (condHAtt.UseOrLogic)
                enabled = enabled || prop2Enabled;
            else
                enabled = enabled && prop2Enabled;
        }
        else
        {
            //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        string[] conditionalSourceFieldArray = condHAtt.ConditionalSourceFields;
        bool[] conditionalSourceFieldInverseArray = condHAtt.ConditionalSourceFieldInverseBools;
        for (int index = 0; index < conditionalSourceFieldArray.Length; ++index)
        {
            conditionPath = propertyPath.Replace(property.name, conditionalSourceFieldArray[index]);
            var sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionPath);
            if (sourcePropertyValueFromArray == null) {
                //original implementation (doens't work with nested serializedObjects)
                sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionalSourceFieldArray[index]);
            }

            if (sourcePropertyValueFromArray != null)
            {
                bool propertyEnabled = CheckPropertyType(sourcePropertyValueFromArray, condHAtt.ConditionalEnumValue);                
                if (conditionalSourceFieldInverseArray.Length>= (index+1) && conditionalSourceFieldInverseArray[index]) propertyEnabled = !propertyEnabled;

                if (condHAtt.UseOrLogic)
                    enabled = enabled || propertyEnabled;
                else
                    enabled = enabled && propertyEnabled;
            }
            else
            {
                //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }
        }

        if (condHAtt.Inverse) enabled = !enabled;

        return enabled;
    }

    private static bool CheckPropertyType(SerializedProperty sourcePropertyValue, int conditionalEnumValue)
    {
        //Note: add others for custom handling if desired
        switch (sourcePropertyValue.propertyType)
        {                
            case SerializedPropertyType.Boolean:
                return !sourcePropertyValue.boolValue;                
            case SerializedPropertyType.ObjectReference:
                return sourcePropertyValue.objectReferenceValue != null;
            case SerializedPropertyType.Enum:
                return sourcePropertyValue.enumValueIndex != conditionalEnumValue;
            default:
                Debug.LogError("Data type of the property used for conditional hiding [" + sourcePropertyValue.propertyType + "] is currently not supported");
                return true;
        }
    }
}
