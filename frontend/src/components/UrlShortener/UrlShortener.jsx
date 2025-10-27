import { useState } from "react";
import axios from "axios";
import "./UrlShortener.scss";

function App() {
  const [url, setUrl] = useState("");
  const [shortUrl, setShortUrl] = useState("");

  const handleSubmit = async e => {
    e.preventDefault();
    const res = await axios.post(
      "http://localhost:5124/api/shorten?url=" + encodeURIComponent(url)
    );
    setShortUrl(res.data.shortUrl);
  };

  return (
    <div className="app-container">
      <h1>URL Shortener</h1>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={url}
          onChange={e => setUrl(e.target.value)}
          placeholder="Enter your URL"
        />
        <button type="submit">Shorten</button>
      </form>
      {shortUrl && (
        <p>
          Shortened URL: <a href={shortUrl}>{shortUrl}</a>
        </p>
      )}
    </div>
  );
}

export default App;
