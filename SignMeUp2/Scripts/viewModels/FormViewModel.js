function InfoFieldViewModel(falt) {
    var self = this;
    var _falt = falt;

    self.type = "information";

    self.information = ko.observable("Inforation");

    if (falt != null) {
        self.information(_falt.Namn);
    }

    self.title = ko.computed(function () {
        return self.information;
    });
};

function TextFieldViewModel(falt) {
    var self = this;
    var _falt = falt;

    self.type = "text";

    self.required = ko.observable(false);

    self.label = ko.observable("Inget namn");

    if (falt != null) {
        self.label(_falt.Namn);

        self.required(falt.Kravs);
    }

    self.title = ko.computed(function () {
        return self.label;
    });
};

function StepViewModel(steg) {
    var self = this;
    var _steg = steg;

    self.namn = ko.observable("Inget namn");

    self.fields = ko.observableArray();

    if (_steg != null && _steg.FaltLista != null) {
        self.namn(_steg.Namn);

        for (i = 0; i < _steg.FaltLista.length; i++) {
            var falt = _steg.FaltLista[i];
            switch (falt.Typ) {
                case 0: // Text
                    self.fields.push(new TextFieldViewModel(falt));
                    break;
                case 1: // Val
                    self.fields.push(new TextFieldViewModel(falt));
                    break;
                case 2: // E-post
                    self.fields.push(new TextFieldViewModel(falt));
                    break;
                case 3: // Info
                    self.fields.push(new InfoFieldViewModel(falt));
                    break;
            }
        }
    }
    
    self.addField = function (type) {
        if (type === "information") {
            self.fields.push(new InfoFieldViewModel());
        } else if (type === "text") {
            self.fields.push(new TextFieldViewModel());
        }
    };
};

function FormViewModel(data) {
    var self = this;

    self.Namn = ko.observable("Namn");

    self.steps = ko.observableArray();

    self.StepIndex = 0;

    if (data != null) {
        self.Namn(data.Namn);

        self.StepIndex = data.StepIndex;

        for (j = 0; j < data.Steg.length; j++) {
            self.steps.push(new StepViewModel(data.Steg[j]));
        }
    }

    self.addStep = function (step) {
        if (step == null) {
            var newIndex = self.steps().length + 1;
            self.steps.push(new StepViewModel(null, newIndex));
            alert('push index: ' + newIndex);
        }
        else {
            self.steps.splice(step.StepIndex, 0, new StepViewModel(null, step.StepIndex));
            alert('splice index: ' + step.StepIndex);
        }
    };

    self.removeStep = function (step) {
        self.steps.remove(step);
    }
};

// TODO, detta verkar inte funger...
function initKO() {

    ko.bindingHandlers.inline = {
        init: function (element, valueAccessor) {
            var span = $(element);
            var input = $('<input />', { 'type': 'text', 'style': 'display:none' });
            span.after(input);

            ko.applyBindingsToNode(input.get(0), { value: valueAccessor() });
            ko.applyBindingsToNode(span.get(0), { text: valueAccessor() });

            span.click(function () {
                input.width(span.width());
                span.hide();
                input.show();
                input.focus();
            });

            input.blur(function () {
                span.show();
                input.hide();
            });

            input.keypress(function (e) {
                if (e.keyCode == 13) {
                    span.show();
                    input.hide();
                };
            });
        }
    };

    //ko.bindingHandlers.contenteditable = {
    //    init: function (element, valueAccessor, allBindingsAccessor) {
    //        var value = ko.utils.unwrapObservable(valueAccessor());
    //        $(element).html(value);
    //        alert('Init!');
    //    },

    //    update: function (element, valueAccessor) {
    //        var value = ko.utils.unwrapObservable(valueAccessor());
    //        if ((value === null) || (value === undefined)) {
    //            value = "";
    //        }
    //        alert('Update!!');
    //    }
    //};
};
