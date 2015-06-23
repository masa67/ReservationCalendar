
/*global angular */
(function () {
    'use strict';

    angular
        .module('GlobalDialogs', [])
        .factory('globalDialogs', function () {

            var GlobalDialogs = (function () {
                var dialogs = {}, Dialog;

                Dialog = (function () {
                    var type = 'BaseHelpers.Dialog',
                        functionNames = ['show', 'hide', 'isVisible'],
                        constructorFunction;

                    function checkSettings(settings) {
                        var errors = [],
                            dialogName = 'dialogName';

                        if (!settings[dialogName]) {
                            errors.push(dialogName + ' is not provided.');
                        }

                        functionNames.forEach(function (functionName) {
                            if (!settings[functionName]) {
                                errors.push(functionName + ' is not defined.');
                            }
                        });

                        if (errors.length) {
                            errors.unshift(type + ' was called with incorrect parameters: ');
                        }

                        return errors.join('');
                    }

                    constructorFunction = function (settings) {
                        var settingsErrors = checkSettings(settings);
                        if (settingsErrors !== '') {
                            throw new Error(settingsErrors);
                        }

                        this.settings = settings;
                    };

                    constructorFunction.prototype.show = function () {
                        return this.settings.show.apply(this, arguments);
                    };

                    constructorFunction.prototype.hide = function () {
                        return this.settings.hide.apply(this, arguments);
                    };

                    constructorFunction.prototype.isVisible = function () {
                        return this.settings.isVisible.apply(this, arguments);
                    };

                    return constructorFunction;
                }());

                return {
                    add: function (settings) {
                        if (!dialogs[settings.dialogName]) {
                            dialogs[settings.dialogName] = new Dialog(settings);
                        }
                    },
                    dialogs: dialogs
                };
            }());

            return GlobalDialogs;
        });
}());