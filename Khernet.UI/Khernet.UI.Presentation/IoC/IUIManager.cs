﻿using System;
using System.Threading.Tasks;

namespace Khernet.UI.IoC
{
    public interface IUIManager
    {
        /// <summary>
        /// Shows a single message dialog
        /// </summary>
        /// <param name="dialogModel">View model for message dialog</param>
        /// <returns></returns>
        Task ShowMessageBox(MessageBoxViewModel dialogModel);

        /// <summary>
        /// Show an modal dialog
        /// </summary>
        /// <typeparam name="T">The type of view model for dialog</typeparam>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowDialog<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Show an child modal dialog owned by parent modial dialog
        /// </summary>
        /// <typeparam name="T">The type of view model for dialog</typeparam>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowChildDialog<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Show system dialog to open files
        /// </summary>
        /// <returns></returns>
        string[] ShowOpenFileDialog();

        /// <summary>
        /// Shows system dialog to save file
        /// </summary>
        /// <returns></returns>
        string ShowSaveFileDialog(string filename = null);

        /// <summary>
        /// Shows system dialog to save file with given filter and default extension.
        /// </summary>
        /// <returns></returns>
        string ShowSaveFileDialog(string filter, string defaultExtension);

        /// <summary>
        /// Opens audio player into main window
        /// </summary>
        /// <typeparam name="T">The type view model</typeparam>
        /// <param name="viewModel">The view model for player</param>
        void ShowPlayer<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Open a file with the defualt application.
        /// </summary>
        /// <param name="fileName">The path of file</param>
        void OpenFile(string fileName);

        /// <summary>
        /// Opens a specified chat
        /// </summary>
        /// <typeparam name="T">The type of view model</typeparam>
        /// <param name="viewModel">The view model of contact to chat</param>
        void ShowChat<T>(T viewModel) where T : BaseModel;

        /// <summary>
        /// Indicates if it is design time
        /// </summary>
        /// <returns>True if it is design time otherwise false</returns>
        bool IsInDesignTime();

        /// <summary>
        /// Converts a <see cref="string"/> to XAML document byte array
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        byte[] ConvertStringToDocument(string value);

        /// <summary>
        /// Converts a <see cref="string"/> html document to XAML document byte array
        /// </summary>
        /// <param name="value">The html document</param>
        /// <returns>A <see cref="byte[]"/> array</returns>
        byte[] ConvertHtmlToDocument(string html);

        /// <summary>
        /// Converts markdown text to XAML document byte array
        /// </summary>
        /// <param name="markdownText"></param>
        /// <returns></returns>
        byte[] ConvertMarkdownToDocument(string markdownText);

        /// <summary>
        /// Shows a notification ballon
        /// </summary>
        /// <param name="notificationModel"></param>
        void ShowNotification(NotificationViewModel notificationModel);

        /// <summary>
        /// Display unread chat messages
        /// </summary>
        void ShowUnReadMessage(int idMessage);

        /// <summary>
        /// Indicates if main window is visible or active
        /// </summary>
        bool IsMainWindowActive();

        /// <summary>
        /// Shows windows if it is not visible
        /// </summary>
        void ShowWindow();

        /// <summary>
        /// Execute a task asynchronously on user interface thread
        /// </summary>
        void ExecuteAsync(Action action);

        /// <summary>
        /// Execute a task synchronously on user interface thread
        /// </summary>
        void Execute(Action action);
    }
}