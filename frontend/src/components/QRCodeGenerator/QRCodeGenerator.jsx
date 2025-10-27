import { QRCodeSVG } from "qrcode.react";
import "./QRCodeGenerator.scss";

export default function QRCodeGenerator({ url }) {
  if (!url) return null;

  return (
    <div className="qr-container">
      <p>
        Shortened URL: <a href={url}>{url}</a>
      </p>
      <QRCodeSVG value={url} />
    </div>
  );
}
