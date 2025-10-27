import { useState } from "react";
import axios from "axios";
import "./UrlShortener.scss";

function UrlShortener({ setShortUrl }) {
  const [url, setUrl] = useState("");

  const handleSubmit = async e => {
    e.preventDefault();
    const res = await axios.post(
      "http://localhost:5124/api/shorten?url=" + encodeURIComponent(url)
    );
    const newUrl = res.data.shortUrl;
    setShortUrl(newUrl);

    // saves links to localStorage
    const stored = JSON.parse(localStorage.getItem("linkHistory")) || [];
    const updated = [{ original: url, shortened: newUrl }, ...stored];
    localStorage.setItem("linkHistory", JSON.stringify(updated));
  };

  return (
    <div className="app-container">
      <h2>Paste URL:</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={url}
          onChange={e => setUrl(e.target.value)}
          placeholder="Enter your URL"
        />
        <button type="submit">Shorten</button>
      </form>
    </div>
  );
}

export default UrlShortener;
