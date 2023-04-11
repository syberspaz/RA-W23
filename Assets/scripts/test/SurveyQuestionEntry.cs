using System.Collections.Generic;

[System.Serializable]
public class SurveyQuestionEntry
{
    //general question data
    public string ID;
    public string Type;
    public int Order;
    public string Text;

    //scale style questions
    public int ScaleMin;
    public int ScaleMax;
    public string ScaleLowLabel;
    public string ScaleHighLabel;

    //option style questions
    public int OptionCount;
    public string Option1;
    public string Option2;
    public string Option3;
    public string Option4;
    public string Option5;
    public string Option6;
    public string Option7;
}
