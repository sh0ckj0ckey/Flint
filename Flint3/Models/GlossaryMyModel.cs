﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Flint3.Models
{
    public class GlossaryColorPercentagePair : ObservableObject
    {
        public GlossaryColorsEnum Color { get; private set; } = GlossaryColorsEnum.Transparent;

        private long _count = 0;
        public long Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        private double _percentage = 0;
        public double Percentage
        {
            get => _percentage;
            set => SetProperty(ref _percentage, value);
        }

        public GlossaryColorPercentagePair(GlossaryColorsEnum color)
        {
            Color = color;
            Count = 0;
            Percentage = 0;
        }
    }

    public class GlossaryMyModel : GlossaryModelBase
    {
        /// <summary>
        /// 生词本的单词颜色比例
        /// </summary>
        private ObservableCollection<GlossaryColorPercentagePair> _wordsColorPercentage = new ObservableCollection<GlossaryColorPercentagePair>();
        public ObservableCollection<GlossaryColorPercentagePair> WordsColorPercentage
        {
            get => _wordsColorPercentage;
            private set => SetProperty(ref _wordsColorPercentage, value);
        }
    }
}