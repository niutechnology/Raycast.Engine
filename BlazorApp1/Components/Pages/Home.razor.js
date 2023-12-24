 //No JavaScript, você precisará de uma função para focar o elemento canvas quando a página for carregada.
 //Isso pode ser feito adicionando o seguinte código em um arquivo .js que é incluído em sua página:
 window.focusElement = (elementId) => {
     document.getElementById(elementId).focus();
 };