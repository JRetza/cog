﻿using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SIL.Cog.Application.Collections;
using SIL.Cog.Domain;
using SIL.ObjectModel;

namespace SIL.Cog.Application.ViewModels
{
	public class SegmentsVarietyViewModel : VarietyViewModel
	{
		private readonly MirroredBindableList<Segment, VarietySegmentViewModel> _segments;
		private readonly ICommand _switchToVarietyCommand;

		public SegmentsVarietyViewModel(SegmentsViewModel segmentsViewModel, Variety variety)
			: base(variety)
		{
			_segments = new MirroredBindableList<Segment, VarietySegmentViewModel>(segmentsViewModel.DomainSegments,
				segment => new VarietySegmentViewModel(this, segment, segmentsViewModel.DomainSyllablePosition), viewModel => viewModel.DomainSegment);
			_switchToVarietyCommand = new RelayCommand(() => Messenger.Default.Send(new SwitchViewMessage(typeof(VarietiesViewModel), DomainVariety)));
		}

		public ReadOnlyObservableList<VarietySegmentViewModel> Segments
		{
			get { return _segments; }
		}

		public ICommand SwitchToVarietyCommand
		{
			get { return _switchToVarietyCommand; }
		}
	}
}
