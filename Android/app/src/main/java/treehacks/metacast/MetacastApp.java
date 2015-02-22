package treehacks.metacast;

import android.app.Activity;
import android.app.ActivityManager;
import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import android.media.AudioManager;
import android.os.Handler;
import android.util.Base64;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.KeyEvent;
import android.view.WindowManager;
import android.widget.Toast;

import com.firebase.client.DataSnapshot;
import com.firebase.client.Firebase;
import com.firebase.client.FirebaseError;
import com.firebase.client.ValueEventListener;

import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.util.List;

import eu.chainfire.libsuperuser.Shell;

public class MetacastApp extends Application {

    Firebase firebase;
    static boolean running = true;

    @Override
    public void onCreate() {
        super.onCreate();

        Firebase.setAndroidContext(this);
        firebase = new Firebase("https://metacast.firebaseio.com/");

        // Screencast
        final AudioManager manager = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
        new Thread(new Runnable() {
            @Override
            public void run() {
                while (running) {
                    try {
                        Shell.SU.run("/system/bin/screencap -p /sdcard/img.png");
                        Bitmap bitmap = BitmapFactory.decodeFile("/sdcard/img.png");
                        bitmap = Bitmap.createScaledBitmap(bitmap, bitmap.getWidth() / 2, bitmap.getHeight() / 2, false);
                        firebase.child("image").setValue(encodeTobase64(bitmap));
                        firebase.child("music").setValue(manager.isMusicActive());
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            }
        }).start();
        // Programmatically clicking
        DisplayMetrics metrics = new DisplayMetrics();
        WindowManager wm = (WindowManager) getSystemService(Context.WINDOW_SERVICE);
        Display display = wm.getDefaultDisplay();
        display.getRealMetrics(metrics);
        final double width = metrics.widthPixels;
        final double height = metrics.heightPixels;
        firebase.child("click").addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                try {
                    Process process = Runtime.getRuntime().exec("su");
                    DataOutputStream os = new DataOutputStream(process.getOutputStream());
                    int x = (int) (Double.parseDouble(dataSnapshot.getValue().toString().split(",")[0]) * width);
                    int y = (int) (Double.parseDouble(dataSnapshot.getValue().toString().split(",")[1]) * height);
                    if (x >= 0 && y >= 0) {
                        String cmd = "/system/bin/input tap " + x + " " + y + "\n";
                        os.writeBytes(cmd);
                        os.writeBytes("exit\n");
                        os.flush();
                        os.close();
                        process.waitFor();
                        firebase.child("click").setValue("-1,-1");
                    }
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }

            @Override
            public void onCancelled(FirebaseError firebaseError) {

            }
        });
        // Launch an app
        firebase.child("app").addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                Intent i;
                String pkg = dataSnapshot.getValue().toString();
                if (!pkg.isEmpty()) {
                    PackageManager manager = getPackageManager();
                    try {
                        i = manager.getLaunchIntentForPackage(pkg);
                        if (i == null)
                            throw new PackageManager.NameNotFoundException();
                        i.addCategory(Intent.CATEGORY_LAUNCHER);
                        startActivity(i);
                        new Handler().postDelayed(new Runnable() {
                            @Override
                            public void run() {
                                refreshRecents();
                            }
                        }, 500);
                    } catch (PackageManager.NameNotFoundException e) {
                        Toast.makeText(getApplicationContext(), "App not found", Toast.LENGTH_SHORT).show();
                    }
                    firebase.child("app").setValue("");
                }
            }

            @Override
            public void onCancelled(FirebaseError firebaseError) {

            }
        });
        // For skipping music
        final Intent i = new Intent(Intent.ACTION_MEDIA_BUTTON);
        firebase.child("command").addValueEventListener(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot dataSnapshot) {
                switch (Integer.parseInt(dataSnapshot.getValue().toString())) {
                    case -1:
                        i.putExtra(Intent.EXTRA_KEY_EVENT, new KeyEvent(KeyEvent.ACTION_DOWN, KeyEvent.KEYCODE_MEDIA_PREVIOUS));
                        sendOrderedBroadcast(i, null);

                        i.putExtra(Intent.EXTRA_KEY_EVENT, new KeyEvent(KeyEvent.ACTION_UP, KeyEvent.KEYCODE_MEDIA_PREVIOUS));
                        sendOrderedBroadcast(i, null);
                        break;
                    case 1:
                        i.putExtra(Intent.EXTRA_KEY_EVENT, new KeyEvent(KeyEvent.ACTION_DOWN, KeyEvent.KEYCODE_MEDIA_NEXT));
                        sendOrderedBroadcast(i, null);

                        i.putExtra(Intent.EXTRA_KEY_EVENT, new KeyEvent(KeyEvent.ACTION_UP, KeyEvent.KEYCODE_MEDIA_NEXT));
                        sendOrderedBroadcast(i, null);
                        break;
                }
                firebase.child("command").setValue(0);
            }

            @Override
            public void onCancelled(FirebaseError firebaseError) {

            }
        });
        refreshRecents();
    }

    private void refreshRecents() {
        ActivityManager am = (ActivityManager) getSystemService(Activity.ACTIVITY_SERVICE);
        List<ActivityManager.RecentTaskInfo> list = am.getRecentTasks(10, 0);
        for (int i = 0; i < Math.min(list.size(), 5); i++) {
            if (list.get(i).baseIntent.getComponent().getPackageName().contains("metacast")
                    || list.get(i).baseIntent.getComponent().getPackageName().contains("googlequicksearchbox")
                    || list.get(i).baseIntent.getComponent().getPackageName().contains("systemui"))
               list.remove(i);
            String pkg = list.get(i).baseIntent.getComponent().getPackageName();
            firebase.child("pkg" + i).setValue(pkg);
            try {
                Drawable icon = getPackageManager().getApplicationIcon(pkg);
                Bitmap bitmap = ((BitmapDrawable) icon).getBitmap();
                firebase.child("icon" + i).setValue(encodeTobase64(bitmap));
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        firebase.child("current_app").setValue(am.getRunningTasks(1).get(0).topActivity.getPackageName());
    }

    @Override
    public void onTerminate() {
        super.onTerminate();
        running = false;
    }

    public static String encodeTobase64(Bitmap image) {
        ByteArrayOutputStream baos = new ByteArrayOutputStream();
        image.compress(Bitmap.CompressFormat.JPEG, 100, baos);
        byte[] b = baos.toByteArray();
        return Base64.encodeToString(b, Base64.DEFAULT);
    }
}
