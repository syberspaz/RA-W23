using System.Collections.Generic;
using System.IO;
using AirtableUnity.PX.Model;
using Newtonsoft.Json;

public static class FileGenerator
{
    public static void WriteAnswerEntryFile(string name, string path, List<BaseRecord<SurveyQuestionEntry>> questions)
    {
        //create
        string filePath = path + "/" + name + ".cs";
        if (!File.Exists(filePath)) File.Create(filePath).Close();

        //open
        StreamWriter file = new StreamWriter(filePath);
        file.WriteLine("public class " + name);
        file.WriteLine("{");
        file.WriteLine("\tpublic int ID;");

        //insert data
        for (int i = 0; i < questions.Count; i++)
        {
            SurveyQuestionEntry entry = questions[i].fields;

            switch (entry.Type)
            {
                case "Page - Select":
                    break;
                case "Page - Scale":
                    break;
                case "Page - Dropdown":
                    break;
                case "Scale":
                    file.WriteLine("\tpublic int " + entry.ID + ";");
                    break;
                case "Select One":
                    file.WriteLine("\tpublic string " + entry.ID + ";");
                    break;
                case "Dropdown":
                    file.WriteLine("\tpublic string " + entry.ID + ";");
                    break;
            }
        }

        //close
        file.WriteLine("}");
        file.Close();
    }

    public static void CreateSurveyJSON(string name, string path, string instructions, string endMessage, string acknowledgements, List<BaseRecord<SurveyQuestionEntry>> questions)
    {
        //create
        string filePath = path + "/" + name + ".json";
        if (!File.Exists(filePath)) File.Create(filePath).Close();

        //open
        StreamWriter file = new StreamWriter(filePath);
        file.WriteLine("{");

        //general
        file.WriteLine("\"qTitle\": \"" + name + "\",");
        file.WriteLine("\"qInstructions\": \"" + instructions + "\",");
        file.WriteLine("\"qId\": \"" + name + "\",");
        file.WriteLine("\"questions\": [");

        //insert data
        bool previousWasPage = false;
        for(int i = 0; i < questions.Count; i++)
        {
            SurveyQuestionEntry entry = questions[i].fields;

            if (entry.Type.Contains("Page"))
            {
                SurveyPageEntryJSON(file, entry, i);
                previousWasPage = true;
            }
            else if (entry.Type == "Scale")
            {
                SurveyScaleQuestionJSON(file, entry, previousWasPage);
                previousWasPage = false;
            }
            else if (entry.Type == "Select One")
            {
                SurveySelectOneQuestionJSON(file, entry, previousWasPage);
                previousWasPage = false;
            }
            else if (entry.Type == "Dropdown")
            {
                SurveyDropdownQuestionJSON(file, entry, previousWasPage);
                previousWasPage = false;
            }
        }

        //close
        file.WriteLine("]");
        file.WriteLine("}"); //close the last page
        file.WriteLine("],"); //close the array
        file.WriteLine("\"qMessage\": \"" + endMessage + "\",");
        file.WriteLine("\"qAcknowledgments\": \"" + acknowledgements + "\"");
        file.WriteLine("}");
        file.Close();

        ConvertFileToPrettyJSON(filePath);
    }

    private static void SurveyPageEntryJSON(StreamWriter file, SurveyQuestionEntry entry, int index)
    {
        //the very first page
        if (index == 0)
        {
            file.WriteLine("{"); //open a new page
            file.WriteLine("\"pId\": \"" + entry.ID + "\",");
            if(entry.Type == "Page - Select") file.WriteLine("\"qType\": \"radio\",");
            else if (entry.Type == "Page - Scale") file.WriteLine("\"qType\": \"linearSlider\",");
            else if (entry.Type == "Page - Dropdown") file.WriteLine("\"qType\": \"dropdown\",");
            file.WriteLine("\"qData\": [");
        }
        else
        {
            file.WriteLine("]");
            file.WriteLine("},"); //close the previous page
            file.WriteLine("{"); //open a new page
            file.WriteLine("\"pId\": \"" + entry.ID + "\",");
            if (entry.Type == "Page - Select") file.WriteLine("\"qType\": \"radio\",");
            else if (entry.Type == "Page - Scale") file.WriteLine("\"qType\": \"linearSlider\",");
            else if (entry.Type == "Page - Dropdown") file.WriteLine("\"qType\": \"dropdown\",");
            file.WriteLine("\"qData\": [");
        }
    }

    private static void SurveyScaleQuestionJSON(StreamWriter file, SurveyQuestionEntry entry, bool previousWasPage)
    {
        if (!previousWasPage) file.Write(",");
        file.WriteLine("{");
        file.WriteLine("\"qId\": \"" + entry.ID + "\",");
        file.WriteLine("\"qText\": \"" + entry.Text + "\",");
        file.WriteLine("\"qMin\": \"" + entry.ScaleMin.ToString() + "\",");
        file.WriteLine("\"qMinLabel\": \"" + entry.ScaleLowLabel + "\",");
        file.WriteLine("\"qMax\": \"" + entry.ScaleMax.ToString() + "\",");
        file.WriteLine("\"qMaxLabel\": \"" + entry.ScaleHighLabel + "\",");
        file.WriteLine("\"qMandatory\": \"true\"");
        file.WriteLine("}");
    }

    private static void SurveySelectOneQuestionJSON(StreamWriter file, SurveyQuestionEntry entry, bool previousWasPage)
    {
        if (!previousWasPage) file.Write(",");
        file.WriteLine("{");
        file.WriteLine("\"qId\": \"" + entry.ID + "\",");
        file.WriteLine("\"qText\": \"" + entry.Text + "\",");
        file.WriteLine("\"qMandatory\": \"true\",");
        file.WriteLine("\"qOptions\": [");

        //This is like this because VR questionaire uses blank "" options on both sides to represent not having those options
        int optionCount = entry.OptionCount;
        int optionsFilled = 0;
        if(optionCount > 0)
        {
            for (int i = 0; i < (7 - optionCount) / 2; i++)
            {
                file.WriteLine("\"\",");
                optionsFilled++;
            }

            if (optionCount >= 1)
            {
                file.WriteLine("\"" + entry.Option1 + "\",");
                optionsFilled++;
            }
            if (optionCount >= 2)
            {
                file.WriteLine("\"" + entry.Option2 + "\",");
                optionsFilled++;
            }
            if (optionCount >= 3)
            {
                file.WriteLine("\"" + entry.Option3 + "\",");
                optionsFilled++;
            }
            if (optionCount >= 4)
            {
                file.WriteLine("\"" + entry.Option4 + "\",");
                optionsFilled++;
            }
            if (optionCount >= 5)
            {
                file.WriteLine("\"" + entry.Option5 + "\",");
                optionsFilled++;
            }
            if (optionCount >= 6)
            {
                file.WriteLine("\"" + entry.Option6 + "\",");
                optionsFilled++;
            }
            if (optionCount >= 7)
            {
                file.WriteLine("\"" + entry.Option7 + "\"");
                optionsFilled++;
            }

            for(;optionsFilled < 7; optionsFilled++)
            {
                if(optionsFilled < 6)file.WriteLine("\"\",");
                else file.WriteLine("\"\"");
            }
        }

        file.WriteLine("]");
        file.WriteLine("}");
    }

    private static void SurveyDropdownQuestionJSON(StreamWriter file, SurveyQuestionEntry entry, bool previousWasPage)
    {
        if (!previousWasPage) file.Write(",");
        file.WriteLine("{");
        file.WriteLine("\"qId\": \"" + entry.ID + "\",");
        file.WriteLine("\"qText\": \"" + entry.Text + "\",");
        file.WriteLine("\"qMandatory\": \"true\",");
        file.WriteLine("\"qOptions\": [");

        if (entry.OptionCount > 1) file.WriteLine("\"" + entry.Option1 + "\",");
        else if (entry.OptionCount == 1) file.WriteLine("\"" + entry.Option1 + "\"");

        if (entry.OptionCount > 2) file.WriteLine("\"" + entry.Option2 + "\",");
        else if (entry.OptionCount == 2) file.WriteLine("\"" + entry.Option2 + "\"");

        if (entry.OptionCount > 3) file.WriteLine("\"" + entry.Option3 + "\",");
        else if (entry.OptionCount == 3) file.WriteLine("\"" + entry.Option3 + "\"");

        if (entry.OptionCount > 4) file.WriteLine("\"" + entry.Option4 + "\",");
        else if (entry.OptionCount == 4) file.WriteLine("\"" + entry.Option4 + "\"");

        if (entry.OptionCount > 5) file.WriteLine("\"" + entry.Option5 + "\",");
        else if (entry.OptionCount == 5) file.WriteLine("\"" + entry.Option5 + "\"");

        if (entry.OptionCount > 6) file.WriteLine("\"" + entry.Option6 + "\",");
        else if (entry.OptionCount == 6) file.WriteLine("\"" + entry.Option6 + "\"");

        if (entry.OptionCount >= 7) file.WriteLine("\"" + entry.Option7 + "\"");

        file.WriteLine("]");
        file.WriteLine("}");
    }

    public static string JsonPrettify(string json)
    {
        using (var stringReader = new StringReader(json))
        using (var stringWriter = new StringWriter())
        {
            var jsonReader = new JsonTextReader(stringReader);
            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }

    public static void ConvertFileToPrettyJSON(string filePath)
    {
        string JSONString = File.ReadAllText(filePath);
        JSONString.Replace("\n", "");
        string prettified = JsonPrettify(JSONString);

        StreamWriter file = new StreamWriter(filePath);
        file.Write(prettified);
        file.Close();
    }
}