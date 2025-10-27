import UrlShortener from "./components/UrlShortener/UrlShortener";
import "./App.scss";

function App() {
  return (
    <div className="app-wrapper">
      <h1 className="app-header">SLURP</h1>
      <h2 className="app-subheader">SLURP Links URLs Rapidly and Precisely</h2>
      <UrlShortener />
    </div>
  );
}

export default App;
