$(function () {

    function InfoFieldViewModel() {
        var self = this;

        self.type = "information";

        self.information = ko.observable("Inforation");

        self.title = ko.computed(function () {
            return self.information;
        });
    };

    function TextFieldViewModel() {
        var self = this;

        self.type = "text";

        self.required = ko.observable(false);

        self.label = ko.observable("Text");

        self.title = ko.computed(function () {
            return self.label;
        });
    };

    function StepViewModel(newIndex) {
        var self = this;

        self.index = newIndex;

        self.title = ko.observable("");

        self.fields = ko.observableArray();

        self.addField = function (type) {
            if (type === "information") {
                self.fields.push(new InfoFieldViewModel());
            } else if (type === "text") {
                self.fields.push(new TextFieldViewModel());
            }
        };
    };

    function FormViewModel() {
        var self = this;

        self.title = ko.observable("Titel");

        self.steps = ko.observableArray();

        self.addStep = function () {
            var newIndex = self.steps().length + 1;

            self.steps.push(new StepViewModel(newIndex));
        };

        self.removeStep = function (step) {
            self.steps.remove(step);
        }
    };

    ko.applyBindings(new FormViewModel());
});
