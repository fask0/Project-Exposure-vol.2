using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    private string _path;
    private string _yearlyPath;
    private string _dateToday;
    private string _dailyFileName;
    private string _yearlyFileName;

    private FileStream _fileStream;
    private StreamReader _sReader;
    private StreamWriter _sWriter;

    private GameObject _dailyScoreContainer;
    private GameObject _yearlyScoreContainer;
    private List<FileEntry> _dailyHighscores = new List<FileEntry>();
    private List<FileEntry> _yearlyHighscores = new List<FileEntry>();

    public struct FileEntry
    {
        public int id;
        public string date;
        public string time;
        public string name;
        public int score;
        public int achievedLevel;
        public int opinionOnTechnology;
        public int increaseInAwareness;

        public string String
        {
            get { return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", id, date, time, name, score, achievedLevel, opinionOnTechnology, increaseInAwareness); }
        }
    }

    void Start()
    {
        _dateToday = DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;
        _path = "Assets/Highscores/Daily/";
        _yearlyPath = "Assets/Highscores/Yearly/";
        _dailyFileName = "REDive " + _dateToday;
        _yearlyFileName = "REDive " + DateTime.Today.Year;

        _dailyScoreContainer = transform.GetChild(0).GetChild(1).GetChild(1).GetChild(2).GetChild(0).gameObject;
        _yearlyScoreContainer = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(2).GetChild(0).gameObject;

        GetDaily();
        GetYearly();

        _yearlyScoreContainer.transform.parent.parent.gameObject.SetActive(false);
    }

    public List<FileEntry> GetScoresToday(bool sortBeforeReturn)
    {
        List<FileEntry> returnList = new List<FileEntry>();
        if (File.Exists(_path + _dailyFileName + ".txt"))
        {
            _sReader = new StreamReader(_path + _dailyFileName + ".txt");

            while (!_sReader.EndOfStream)
            {
                string line = _sReader.ReadLine();
                if (line.Length > 0)
                {
                    string[] lineValues = line.Split(',');

                    //Created file entry
                    FileEntry fe = new FileEntry();
                    fe.id = Convert.ToInt32(lineValues[0]);
                    fe.date = lineValues[1];
                    fe.time = lineValues[2];
                    fe.name = lineValues[3];
                    fe.score = Convert.ToInt32(lineValues[4]);
                    fe.achievedLevel = Convert.ToInt32(lineValues[5]);
                    fe.opinionOnTechnology = Convert.ToInt32(lineValues[6]);
                    fe.increaseInAwareness = Convert.ToInt32(lineValues[7]);

                    returnList.Add(fe);
                }
            }

            _sReader.Close();

            if (sortBeforeReturn)
                returnList.Sort(new SortFileEntryDescending());
        }

        return returnList;
    }

    public void GetDaily()
    {
        _dailyHighscores = GetScoresToday(true);

        if (_dailyHighscores.Count == 0)
        {
            _dailyScoreContainer.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }

        _dailyScoreContainer.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
        _dailyScoreContainer.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[0].name;
        _dailyScoreContainer.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[0].score.ToString();

        for (int i = 1; i < _dailyHighscores.Count; i++)
        {
            GameObject entry = Instantiate(_dailyScoreContainer.transform.GetChild(0).gameObject, _dailyScoreContainer.transform);

            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[i].name;
            entry.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[i].score.ToString();
        }
    }

    public void ShowDaily()
    {
        _dailyScoreContainer.transform.parent.parent.gameObject.SetActive(true);
        _yearlyScoreContainer.transform.parent.parent.gameObject.SetActive(false);
    }

    public List<FileEntry> GetScoresYearly(bool sortBeforeReturn)
    {
        List<FileEntry> returnList = new List<FileEntry>();
        if (File.Exists(_yearlyPath + _yearlyFileName + ".txt"))
        {
            _sReader = new StreamReader(_yearlyPath + _yearlyFileName + ".txt");

            while (!_sReader.EndOfStream)
            {
                string line = _sReader.ReadLine();
                if (line.Length > 0)
                {
                    string[] lineValues = line.Split(',');

                    //Created file entry
                    FileEntry fe = new FileEntry();
                    fe.id = Convert.ToInt32(lineValues[0]);
                    fe.date = lineValues[1];
                    fe.time = lineValues[2];
                    fe.name = lineValues[3];
                    fe.score = Convert.ToInt32(lineValues[4]);
                    fe.achievedLevel = Convert.ToInt32(lineValues[5]);
                    fe.opinionOnTechnology = Convert.ToInt32(lineValues[6]);
                    fe.increaseInAwareness = Convert.ToInt32(lineValues[7]);

                    returnList.Add(fe);
                }
            }

            _sReader.Close();

            if (sortBeforeReturn)
                returnList.Sort(new SortFileEntryDescending());
        }

        return returnList;
    }

    public void GetYearly()
    {
        _yearlyHighscores = GetScoresYearly(true);

        if (_yearlyHighscores.Count == 0)
        {
            _yearlyScoreContainer.transform.GetChild(0).gameObject.SetActive(false);
            return;
        }

        _yearlyScoreContainer.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
        _yearlyScoreContainer.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[0].name;
        _yearlyScoreContainer.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[0].score.ToString();

        for (int i = 1; i < _yearlyHighscores.Count; i++)
        {
            GameObject entry = Instantiate(_yearlyScoreContainer.transform.GetChild(0).gameObject, _yearlyScoreContainer.transform);

            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[i].name;
            entry.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[i].score.ToString();
        }
    }

    public void ShowYearly()
    {
        _yearlyScoreContainer.transform.parent.parent.gameObject.SetActive(true);
        _dailyScoreContainer.transform.parent.parent.gameObject.SetActive(false);
    }

    private class SortFileEntryDescending : IComparer<FileEntry>
    {
        int IComparer<FileEntry>.Compare(FileEntry a, FileEntry b)
        {
            if (a.score > b.score)
                return -1;
            if (a.score < b.score)
                return 1;
            else
                return 0;
        }
    }
}
