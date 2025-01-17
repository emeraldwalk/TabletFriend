﻿using System;
using System.IO;
using TabletFriend.Models;

namespace TabletFriend
{
	public static class AppState
	{
		public static readonly string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

		public static readonly string FilesRoot = Path.Combine(CurrentDirectory, "files");
		public static readonly string LayoutRoot = Path.Combine(CurrentDirectory, "files\\layouts");

		public const string LayoutExtension = "*.yaml";

		public static string[] Layouts;

		public static LayoutModel CurrentLayout;

		public static string CurrentLayoutPath;

		public static Settings Settings;
	}
}
