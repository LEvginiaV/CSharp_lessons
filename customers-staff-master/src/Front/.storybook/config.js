import {configure} from '@storybook/react';

const req = require.context('../src/', true, /.stories.(jsx|tsx)$/);

function loadStories() {
  req.keys().forEach(filename => req(filename));
}

window.IS_DEMO_MODE = true;

configure(loadStories, module);
