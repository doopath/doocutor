﻿using CUI.ColorSchemes;

namespace CUI
{
    public class ColorSchemesRepository
    {
        private readonly Dictionary<string, IColorScheme> _repository;

        public ColorSchemesRepository()
        {
            _repository = new();
        }

        public void Add(IColorScheme scheme)
            => _repository.Add(scheme.Name, scheme);

        public IColorScheme Get(string schemeName)
            => _repository[schemeName];
    }
}