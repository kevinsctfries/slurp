import UrlShortener from "./components/UrlShortener/UrlShortener";
import "./App.scss";
import QRCodeGenerator from "./components/QRCodeGenerator/QRCodeGenerator";
import { useState } from "react";

function App() {
  const [shortUrl, setShortUrl] = useState("");

  return (
    <div className="app-wrapper">
      <h1 className="app-header">SLURP</h1>
      <h2 className="app-subheader">SLURP Links URLs Rapidly and Precisely</h2>
      <UrlShortener setShortUrl={setShortUrl} />

      {shortUrl && (
        <>
          <QRCodeGenerator url={shortUrl} />
        </>
      )}
    </div>
  );
}

export default App;
