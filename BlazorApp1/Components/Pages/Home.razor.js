 //No JavaScript, voc� precisar� de uma fun��o para focar o elemento canvas quando a p�gina for carregada.
 //Isso pode ser feito adicionando o seguinte c�digo em um arquivo .js que � inclu�do em sua p�gina:
 window.focusElement = (elementId) => {
     document.getElementById(elementId).focus();
 };