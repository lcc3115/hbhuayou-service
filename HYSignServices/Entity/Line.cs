using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HYSignServices.Entity
{
    public class Line
    {
        double a;
        double b;
        double c;
        Line(double a,double b,double c){
            this.a=a;
            this.b=b;
            this.c=c;
        }
        public static Line getLine(Points p1,Points p2){
            //cy=ax+b
            double a=(p1.y-p2.y)/(p1.x-p2.x),b,c;
            //          if(Double.isNaN(a))throw new NumberFormatException("输入的两点有重合");
            //          else
            if(Double.IsInfinity(a)){
                a=1;
                b=-p1.x;
                c=0;
            }else{
                b=(p1.x*p2.y-p2.x*p1.y)/(p1.x-p2.x);
                c=1;
            }
            return new Line(a,b,c);
        }
        public static double getValue(Line l,Points p){
            return l.a*p.x+l.b-l.c*p.y;
        }
    }

    public class Points{
        public double x;
        public double y;
        public Points(double x,double y){
            this.x=x;
            this.y=y;
        }
        public double getX() {
            return x;
        }
        public void setX(double x) {
            this.x = x;
        }
        public double getY() {
            return y;
        }
        public void setY(double y) {
            this.y = y;
        }

    }

}