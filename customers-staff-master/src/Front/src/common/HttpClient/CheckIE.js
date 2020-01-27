export default function isIE() {
  var ua = window.navigator.userAgent;
  var msie = ua.indexOf('MSIE ');
  var trident = ua.indexOf('Trident/');
  var edge = ua.indexOf('Edge/');
  if (msie > 0) {
    return true;
  }
  else if (trident > 0) {
    return true;
  }
  else if (edge > 0) {
    return true;
  }
  else
    return false;
}

export function ieVersion() {
  const ua = window.navigator.userAgent;
  if (ua.indexOf("Trident/7.0") > 0)
    return 11;
  else if (ua.indexOf("Trident/6.0") > 0)
    return 10;
  else if (ua.indexOf("Trident/5.0") > 0)
    return 9;
  else if (ua.indexOf("Trident/4.0") > 0)
    return 8;
  else
    return 0;
}
