import SuperAgent from 'superagent';
import noCache from './SuperagentNoCache';
import UAParser from 'ua-parser-js';

const parser = new UAParser();
const {browser} = parser.getResult();

const SCRIPTS_BUILD_TIME = "SCRIPTS_BUILD_TIME";

function getReject(error) {
  if (error === undefined || error.status === 200) {
    return new Error();
  }

  if (error.status === 400 && error.response && error.response.body) {
    const vrError = new Error(error.response.text);
    vrError["validationResult"] = error.response.body;
    return vrError;
  }

  return new Error(`statusCode is ${error.status}`);
}

function _get(url, queryJson) {
  return new Promise((resolve, reject) => {
    const request = SuperAgent
      .get(url)
      .set("Client-Url", location.pathname)
      .set("React-Url", location.hash)
      .set("Browser-Name", browser.name)
      .set("Browser-Version", browser.version)
      .set("Scripts-Build-Time", SCRIPTS_BUILD_TIME)
      .use(noCache)
      .query(queryJson)
      .end(function (err, res) {
        if (err) {
          reject(err);
        }
        else {
          resolve(res);
        }
      })
  }).catch(error => Promise.reject(getReject(error)))
    .then(res => res.body);
}

function _downloadFileAsText(url, queryJson) {
  return new Promise((resolve, reject) => {
    const request = SuperAgent
      .get(url)
      .set("Client-Url", location.pathname)
      .set("React-Url", location.hash)
      .set("Browser-Name", browser.name)
      .set("Browser-Version", browser.version)
      .set("Scripts-Build-Time", SCRIPTS_BUILD_TIME)
      .use(noCache)
      .query(queryJson)
      .end(function (err, res) {
        if (err) {
          reject(err);
        }
        else {
          resolve(res);
        }
      })
  }).catch(error => Promise.reject(getReject(error)))
    .then(res => res.text);
}

async function _download(url, queryJson) {
  await _head(url, queryJson);
  window.location = url;
}


function _post(url, dataJson) {
  return new Promise((resolve, reject) => {
    const request = SuperAgent
      .post(url)
      .set("Client-Url", location.pathname)
      .set("React-Url", location.hash)
      .set("Browser-Name", browser.name)
      .set("Browser-Version", browser.version)
      .set("Scripts-Build-Time", SCRIPTS_BUILD_TIME)
      .use(noCache)
      .send(dataJson)
      .end(function (err, res) {
        if (err) {
          reject(err);
        }
        else {
          resolve(res);
        }
      })
  }).catch(error => Promise.reject(getReject(error)))
    .then(res => res.body);
}

function _put(url, dataJson) {
  return new Promise((resolve, reject) => {
    const request = SuperAgent
      .put(url)
      .set("Client-Url", location.pathname)
      .set("React-Url", location.hash)
      .set("Browser-Name", browser.name)
      .set("Browser-Version", browser.version)
      .set("Scripts-Build-Time", SCRIPTS_BUILD_TIME)
      .use(noCache)
      .send(dataJson)
      .end(function (err, res) {
        if (err) {
          reject(err);
        }
        else {
          resolve(res);
        }
      })
  }).catch(error => Promise.reject(getReject(error)))
    .then(res => res.body);
}

function _head(url, queryJson) {
  return new Promise((resolve, reject) => {
    const request = SuperAgent
      .head(url)
      .set("Client-Url", location.pathname)
      .set("React-Url", location.hash)
      .set("Browser-Name", browser.name)
      .set("Browser-Version", browser.version)
      .set("Scripts-Build-Time", SCRIPTS_BUILD_TIME)
      .use(noCache)
      .query(queryJson)
      .end(function (err, res) {
        if (err) {
          reject(err);
        }
        else {
          resolve(res);
        }
      })
  }).catch(error => Promise.reject(getReject(error)))
    .then(res => Promise.resolve());
}

function _upload(url, files) {
  return new Promise((resolve, reject) => {
    const request = SuperAgent
      .post(url)
      .set("Client-Url", location.pathname)
      .set("React-Url", location.hash)
      .set("Browser-Name", browser.name)
      .set("Browser-Version", browser.version)
      .set("Scripts-Build-Time", SCRIPTS_BUILD_TIME)
      .use(noCache);
    files.forEach(file => {
      request.attach('file', file);
    });
    request.end((err, res) => {
      if (err) {
        reject(err);
      }
      else {
        resolve(res);
      }
    });
  }).catch(error => Promise.reject(getReject(error)))
    .then(res => res.body);
}

function _delete(url, queryJson) {
  return new Promise((resolve, reject) => {
    const request = SuperAgent
      .delete(url)
      .set("Client-Url", location.pathname)
      .set("React-Url", location.hash)
      .set("Browser-Name", browser.name)
      .set("Browser-Version", browser.version)
      .set("Scripts-Build-Time", SCRIPTS_BUILD_TIME)
      .use(noCache)
      .query(queryJson)
      .end(function (err, res) {
        if (err) {
          reject(err);
        }
        else {
          resolve(res);
        }
      })
  }).catch(error => Promise.reject(getReject(error)))
    .then(res => res.body);
}

class HttpClientImpl {
  get(url, queryJson) {
    return _get(url, queryJson);
  }

  post(url, dataJson) {
    return _post(url, dataJson);
  }

  put(url, dataJson) {
    return _put(url, dataJson);
  }

  download(url, queryJson) {
    return _download(url, queryJson);
  }

  downloadFileAsText(url, queryJson) {
    return _downloadFileAsText(url, queryJson);
  }

  head(url, queryJson) {
    return _head(url, queryJson);
  }

  upload(url, files) {
    return _upload(url, files);
  }

  delete(url, files) {
    return _delete(url, files);
  }
}


export default new HttpClientImpl();
