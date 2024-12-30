using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using DirectoryChangeMonitoring.Models;
using System.Diagnostics;

public class HomeController : Controller
{
    private static DirectoryState? _previousState;

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["DebugMessages"] = new List<string>();
        _previousState = null; // Forgets the previous analyze when we change directory
        return View();
    }

    [HttpPost]
    public IActionResult Analyze(string directoryPath)
    {
        var debugMessages = new List<string>();

        // Checks if the directorypath is OK
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            debugMessages.Add("Error: Invalid directory path.");
            ViewData["DebugMessages"] = debugMessages;
            return View("Index",debugMessages);
        }
       
        var currentState = DirectoryState.Load(directoryPath);
        // Update the stored state
        List<FileDetail> newFiles, changedFiles, deletedFiles;
        if (_previousState == null)
        {
            newFiles = new List<FileDetail>();
            changedFiles = new List<FileDetail>();
            deletedFiles = new List<FileDetail>();
            TempData["ShowChanges"] = false;
        }
        else
        {
            (newFiles, changedFiles, deletedFiles) = currentState.Compare(_previousState);
            TempData["ShowChanges"] = true;
        }
        _previousState = currentState;

        ViewBag.NewFiles = newFiles;
        ViewBag.ChangedFiles = changedFiles;
        ViewBag.DeletedFiles = deletedFiles;
        ViewBag.DebugMessages = debugMessages;
        ViewBag.DirectoryPath = directoryPath;

        ViewData["DebugMessages"] = debugMessages;
        return View("Result");
    }
}
