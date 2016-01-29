var Formularsbyggaren = React.createClass({displayName: 'Formulärsbyggaren',
  render: function() {
      return (
      React.createElement('div', {className: "formularsbyggaren"},
        "Hello, world! I am a F."
      )
    );
  }
});
ReactDOM.render(
  React.createElement(Formularsbyggaren, null),
  document.getElementById('content')
);