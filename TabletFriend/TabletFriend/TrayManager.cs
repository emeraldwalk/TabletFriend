﻿using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TabletFriend
{
	public class TrayManager
	{
		private class TrayCommand : ICommand
		{
			// Why do we need an entire class instead of an event handler? No fucking idea.
			public event EventHandler CanExecuteChanged;
			public bool CanExecute(object parameter) => true;
			public void Execute(object parameter) => EventBeacon.SendEvent("toggle_minimize");
		}


		private TaskbarIcon _icon;
		private MenuItem _autostartMenuItem;
		private readonly string _iconPathBlack = AppState.CurrentDirectory + "/files/icons/tray/tray_black.ico";
		private readonly string _iconPathWhite = AppState.CurrentDirectory + "/files/icons/tray/tray_white.ico";

		public TrayManager(LayoutListManager layoutList)
		{
			_icon = new TaskbarIcon();

			if (_isLightTheme)
			{
				_icon.Icon = new Icon(_iconPathBlack);
			}
			else
			{ 
				_icon.Icon = new Icon(_iconPathWhite);
			}

			_icon.Visibility = Visibility.Visible;
			_icon.ContextMenu = new ContextMenu();
			_icon.LeftClickCommand = new TrayCommand();

			_icon.ContextMenu.Items.Add(layoutList.Menu);

			if (AppState.Settings.AddToAutostart)
			{
				_autostartMenuItem = AddMenuItem("remove from autostart", OnAutostartToggle);
			}
			else
			{
				_autostartMenuItem = AddMenuItem("add to autostart", OnAutostartToggle);
			}

			AddMenuItem("open layouts directory...", OnOpenLayoutsDirectory);
			AddMenuItem("about", OnAbout);
			AddMenuItem("quit", OnQuit);
		}

		private void OnAutostartToggle(object sender, RoutedEventArgs e)
		{
			AppState.Settings.AddToAutostart = !AppState.Settings.AddToAutostart;

			if (AppState.Settings.AddToAutostart)
			{
				AutostartManager.SetAutostart();
				_autostartMenuItem.Header = "remove from autostart";
			}
			else
			{
				AutostartManager.ResetAutostart();
				_autostartMenuItem.Header = "add to autostart";
			}
			EventBeacon.SendEvent("update_settings");

		}

		private void OnAbout(object sender, RoutedEventArgs e)
		{
			var startInfo = new ProcessStartInfo()
			{
				Arguments = AppState.LayoutRoot,
				FileName = "http://github.com/Martenfur/TabletFriend",
				UseShellExecute = true,
			};
			Process.Start(startInfo);
		}

		private MenuItem AddMenuItem(string header, RoutedEventHandler click = null)
		{
			var item = new MenuItem() { Header = header };
			if (click != null)
			{
				item.Click += click;
			}
			_icon.ContextMenu.Items.Add(item);
			return item;
		}


		private void OnOpenLayoutsDirectory(object sender, RoutedEventArgs e)
		{
			var startInfo = new ProcessStartInfo()
			{
				Arguments = AppState.LayoutRoot,
				FileName = "explorer.exe"
			};
			Process.Start(startInfo);
		}


		private void OnQuit(object sender, RoutedEventArgs e)
		{
			_icon.Dispose();
			Application.Current.Shutdown();
		}

		private bool _isLightTheme
		{
			get
			{
				var key = Registry.GetValue(
					@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", 
					"SystemUsesLightTheme", 
					0
				);
				return !(key == null || (int)key == 0);
			}
		}

	}
}
