﻿using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace RezPls
{
    public class RezPls : IDalamudPlugin
    {
        public string Name
            => "RezPls";

        private DalamudPluginInterface? _pluginInterface;
        private ActorWatcher?           _actorWatcher;
        private Overlay?                _overlay;
        private Interface?              _interface;
        private RezPlsConfig?          _config;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pluginInterface = pluginInterface;
            _config          = _pluginInterface.GetPluginConfig() as RezPlsConfig;
            if (_config == null)
            {
                _config = new RezPlsConfig();
                Save();
            }

            _actorWatcher = new ActorWatcher(_pluginInterface);
            _overlay      = new Overlay(_pluginInterface, _actorWatcher, _config);
            _interface    = new Interface(_pluginInterface, this, _config);
            if (_config.Enabled)
                Enable();

            _pluginInterface.CommandManager.AddHandler("/rezpls", new CommandInfo(OnRezPls)
            {
                HelpMessage = "Open the configuration window for RezPls.",
                ShowInHelp  = true,
            });
        }

        public void OnRezPls(string _, string arguments)
        {
            _interface!.Visible = !_interface.Visible;
        }

        public void Save()
            => _pluginInterface!.SavePluginConfig(_config);

        public void Enable()
        {
            _actorWatcher!.Enable();
            _overlay!.Enable();
        }

        public void Disable()
        {
            _actorWatcher!.Disable();
            _overlay!.Disable();
        }

        public void Dispose()
        {
            _pluginInterface!.CommandManager.RemoveHandler("/rezpls");
            _interface?.Dispose();
            _overlay?.Dispose();
            _actorWatcher?.Dispose();
            _pluginInterface?.Dispose();
        }
    }
}