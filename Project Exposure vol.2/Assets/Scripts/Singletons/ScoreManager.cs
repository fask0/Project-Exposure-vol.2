using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private int _maxDailyScoreEntries = 500;
    [SerializeField]
    private int _maxYearlyScoreEntries = 50;

    private string _path;
    private string _yearlyPath;
    private string _dateToday;
    private string _dailyFileName;
    private string _yearlyFileName;

    private FileStream _fileStream;
    private StreamReader _sReader;
    private StreamWriter _sWriter;

    private string _name = "Rimme :)";
    private int _id;
    private int _achievedLevel = 3;
    private int _opinionOnTechnology = 5;
    private int _increaseInAwareness = 5;
    private int _currentScore = 0;
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

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.ScoreManager = this;

        _id = UnityEngine.Random.RandomRange(-int.MaxValue, int.MaxValue);
        _dateToday = DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;
        _path = "Assets/Highscores/Daily/";
        _yearlyPath = "Assets/Highscores/Yearly/";
        _dailyFileName = "REDive " + _dateToday;
        _yearlyFileName = "REDive " + DateTime.Today.Year;

        _dailyScoreContainer = SingleTons.FindChild(SingleTons.FindChild(SingleTons.FindChild(SingleTons.FindChild(MainCanavasManager.ResolutionScreen, "InfoPanel"), "Daily"), "ScrollWindow"), "ScoreContainer");
        _yearlyScoreContainer = SingleTons.FindChild(SingleTons.FindChild(SingleTons.FindChild(SingleTons.FindChild(MainCanavasManager.ResolutionScreen, "InfoPanel"), "Yearly"), "ScrollWindow"), "ScoreContainer");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            List<FileEntry> fileEntries = GetScoresToday(true);
            foreach (FileEntry fileEntry in fileEntries)
            {
                Debug.Log(fileEntry.String);
            }
        }
        if (Input.GetKey(KeyCode.F7))
        {
            SaveScoreToday();
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            ClearFile(_path + _dailyFileName + ".txt");
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            DeleteFile(_path + _dailyFileName + ".txt");
        }
    }

    public int GetScore()
    {
        return _currentScore;
    }

    public void AddScore(int score)
    {
        _currentScore += score;
    }

    public void SetScore(int score)
    {
        _currentScore = score;
    }

    //-----------------------------//
    //                             //
    //   Highscore Functionality   //
    //                             //
    //-----------------------------//
    public void CreateFile(string pPath, string pFileName)
    {
        if (!File.Exists(pPath + pFileName + ".txt"))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(pPath));
            Debug.Log("File does not exist yet.. Creating file " + pFileName + ".txt now");
            _fileStream = new FileStream(pPath + pFileName + ".txt", FileMode.Create);
            _fileStream.Close();
        }
        else
        {
            Debug.Log("File already exists");
        }
    }

    public void DeleteFile(string path)
    {
        File.Delete(path);
        Debug.Log("Deleted file " + path);
    }

    public void ClearFile(string path)
    {
        CloseAll();
        _sWriter = new StreamWriter(path);
        _sWriter.Close();
        Debug.Log("Cleared file " + path);
    }

    public void SaveScoreToday()
    {
        CloseAll();

        if (!File.Exists(_path + _dailyFileName + ".txt"))
            CreateFile(_path, _dailyFileName);

        _sWriter = new StreamWriter(_path + _dailyFileName + ".txt", true);
        _sWriter.WriteLine(string.Format("{0},{1},{2}:{3},{4},{5},{6},{7},{8}", _id, _dateToday, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, _name, _currentScore, _achievedLevel, _opinionOnTechnology, _increaseInAwareness));
        _sWriter.Close();

        Debug.Log(string.Format("Wrote: \"{0},{1},{2}:{3},{4},{5},{6},{7},{8}\" to {9}{10}.txt", _id, _dateToday, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, _name, _currentScore, _achievedLevel, _opinionOnTechnology, _increaseInAwareness, _path, _dailyFileName));

        CloseAll();
        List<FileEntry> fileEntries = GetScoresToday(true);
        if (fileEntries.Count > 500)
        {
            fileEntries.RemoveAt(fileEntries.Count - 1);
        }

        ClearFile(_path + _dailyFileName + ".txt");
        _sWriter = new StreamWriter(_path + _dailyFileName + ".txt", true);
        foreach (FileEntry fileEntry in fileEntries)
        {
            _sWriter.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", fileEntry.id, fileEntry.date, fileEntry.time, fileEntry.name, fileEntry.score, fileEntry.achievedLevel, fileEntry.opinionOnTechnology, fileEntry.increaseInAwareness));
        }
        _sWriter.Close();

        SaveScoreYearly();
    }

    private void SaveScoreYearly()
    {
        CloseAll();

        if (!File.Exists(_yearlyPath + _yearlyFileName + ".txt"))
            CreateFile(_yearlyPath, _yearlyFileName);

        _sWriter = new StreamWriter(_yearlyPath + _yearlyFileName + ".txt", true);
        _sWriter.WriteLine(string.Format("{0},{1},{2}:{3},{4},{5},{6},{7},{8}", _id, _dateToday, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, _name, _currentScore, _achievedLevel, _opinionOnTechnology, _increaseInAwareness));
        _sWriter.Close();

        Debug.Log(string.Format("Wrote: \"{0},{1},{2}:{3},{4},{5},{6},{7},{8}\" to {9}{10}.txt", _id, _dateToday, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, _name, _currentScore, _achievedLevel, _opinionOnTechnology, _increaseInAwareness, _yearlyPath, _yearlyFileName));

        CloseAll();
        List<FileEntry> fileEntries = GetScoresYearly(true);
        if (fileEntries.Count > 50)
        {
            fileEntries.RemoveAt(fileEntries.Count - 1);
        }

        ClearFile(_yearlyPath + _yearlyFileName + ".txt");
        _sWriter = new StreamWriter(_yearlyPath + _yearlyFileName + ".txt", true);
        foreach (FileEntry fileEntry in fileEntries)
        {
            _sWriter.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", fileEntry.id, fileEntry.date, fileEntry.time, fileEntry.name, fileEntry.score, fileEntry.achievedLevel, fileEntry.opinionOnTechnology, fileEntry.increaseInAwareness));
        }
        _sWriter.Close();
    }

    public string ReadScoreToday()
    {
        if (File.Exists(_path + _dailyFileName + ".txt"))
        {
            _sReader = new StreamReader(_path + _dailyFileName + ".txt");

            string returnVal = _sReader.ReadToEnd();
            Debug.Log(returnVal);
            _sReader.Close();

            return returnVal;
        }
        else
        {
            Debug.Log("A score file has yet to be created for today");
            return "";
        }
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

    private void CloseAll()
    {
        if (_fileStream != null)
            _fileStream.Close();
        if (_sWriter != null)
            _sWriter.Close();
        if (_sReader != null)
            _sReader.Close();
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

    private class SortIntDescending : IComparer<int>
    {
        int IComparer<int>.Compare(int a, int b)
        {
            if (a > b)
                return -1;
            if (a < b)
                return 1;
            else
                return 0;
        }
    }

    public void GetDaily()
    {
        _dailyHighscores = GetScoresToday(true);

        _dailyScoreContainer.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
        _dailyScoreContainer.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[0].name;
        _dailyScoreContainer.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[0].score.ToString();
        if (_dailyHighscores[0].id == _id && _dailyHighscores[0].name == _name && _dailyHighscores[0].score == _currentScore)
            _dailyScoreContainer.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = Color.white;
        else
            _dailyScoreContainer.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = new Color(1, 1, 1, 0);

        for (int i = 1; i < _dailyHighscores.Count; i++)
        {
            GameObject entry = Instantiate(_dailyScoreContainer.transform.GetChild(0).gameObject, _dailyScoreContainer.transform);

            if (_dailyHighscores[i].id == _id && _dailyHighscores[i].name == _name && _dailyHighscores[i].score == _currentScore)
                entry.transform.GetChild(3).GetComponent<Image>().color = Color.white;
            else
                entry.transform.GetChild(3).GetComponent<Image>().color = new Color(1, 1, 1, 0);

            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[i].name;
            entry.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _dailyHighscores[i].score.ToString();
        }
    }

    public void ShowDaily()
    {
        _dailyScoreContainer.SetActive(true);
        _yearlyScoreContainer.SetActive(false);
    }

    public void GetYearly()
    {
        _yearlyHighscores = GetScoresYearly(true);

        _yearlyScoreContainer.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
        _yearlyScoreContainer.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[0].name;
        _yearlyScoreContainer.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[0].score.ToString();
        if (_yearlyHighscores[0].id == _id && _yearlyHighscores[0].name == _name && _yearlyHighscores[0].score == _currentScore)
            _yearlyScoreContainer.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = Color.white;
        else
            _yearlyScoreContainer.transform.GetChild(0).GetChild(3).GetComponent<Image>().color = new Color(1, 1, 1, 0);

        for (int i = 1; i < _yearlyHighscores.Count; i++)
        {
            GameObject entry = Instantiate(_yearlyScoreContainer.transform.GetChild(0).gameObject, _yearlyScoreContainer.transform);

            if (_yearlyHighscores[i].id == _id && _yearlyHighscores[i].name == _name && _yearlyHighscores[i].score == _currentScore)
                entry.transform.GetChild(3).GetComponent<Image>().color = Color.white;
            else
                entry.transform.GetChild(3).GetComponent<Image>().color = new Color(1, 1, 1, 0);

            entry.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            entry.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[i].name;
            entry.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _yearlyHighscores[i].score.ToString();
        }
    }

    public void ShowYearly()
    {
        _dailyScoreContainer.SetActive(false);
        _yearlyScoreContainer.SetActive(true);
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void NextQuestion(GameObject pGameobject)
    {
        if (pGameobject.transform.parent.name == "Question0")
        {
            int rating = 0;

            for (int i = 0; i < pGameobject.transform.parent.GetChild(1).GetChild(1).childCount; i++)
                if (pGameobject.transform.parent.GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().enabled)
                    rating = i + 1;

            _opinionOnTechnology = (rating == 0) ? 5 : rating;
        }
        else if (pGameobject.transform.parent.name == "Question1")
        {
            int rating = 0;

            for (int i = 0; i < pGameobject.transform.parent.GetChild(1).GetChild(1).childCount; i++)
                if (pGameobject.transform.parent.GetChild(1).GetChild(1).GetChild(i).GetComponent<Image>().enabled)
                    rating = i + 1;

            _increaseInAwareness = (rating == 0) ? 5 : rating;
            SaveScoreToday();
            GetDaily();
            GetYearly();
        }
    }
}
