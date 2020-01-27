// Взято из Egais/Front/typings

declare global {
  interface Window {
    _paq?: {
      push: (arg: [string, string, string, string, string]) => void,
    },
  }
}
