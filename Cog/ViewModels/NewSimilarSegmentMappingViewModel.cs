﻿using System.ComponentModel;
using SIL.Machine;

namespace SIL.Cog.ViewModels
{
	public class NewSimilarSegmentMappingViewModel : CogViewModelBase, IDataErrorInfo
	{
		private readonly CogProject _project;
		private string _segment1;
		private string _segment2;

		public NewSimilarSegmentMappingViewModel(CogProject project)
			: base("New Mapping")
		{
			_project = project;
		}

		public string Segment1
		{
			get { return _segment1; }
			set { Set(() => Segment1, ref _segment1, value); }
		}

		public string Segment2
		{
			get { return _segment2; }
			set { Set(() => Segment2, ref _segment2, value); }
		}

		public string this[string columnName]
		{
			get
			{
				switch (columnName)
				{
					case "Segment1":
						return GetErrorInfo(_segment1);

					case "Segment2":
						return GetErrorInfo(_segment2);
				}

				return null;
			}
		}

		private string GetErrorInfo(string segment)
		{
			if (string.IsNullOrEmpty(segment))
				return "Please specify a segment";
			Shape shape;
			if (!_project.Segmenter.ToShape(segment, out shape))
				return "This is an invalid segment";
			if (shape.Count > 1)
				return "Please specify only one segment";
			return null;
		}

		public string Error
		{
			get { return null; }
		}
	}
}
